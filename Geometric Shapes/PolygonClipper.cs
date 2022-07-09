// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Freya {

	/// <summary>Utility type to clip polygons</summary>
	public static class PolygonClipper {

		enum PointSideState {
			Discard = -1,
			Edge = 0,
			Keep = 1,
			Handled = 2
		}

		public enum ResultState {
			OriginalLeftIntact,
			Clipped,
			FullyDiscarded
		}

		public class PolygonSection : IComparable<PolygonSection> {
			public FloatRange tRange;
			public List<Vector2> points;
			public PolygonSection( FloatRange tRange, List<Vector2> points ) => ( this.tRange, this.points ) = ( tRange, points );
			public int CompareTo( PolygonSection other ) => tRange.Min.CompareTo( other.tRange.Min );
		}

		static List<PointSideState> states = new List<PointSideState>();

		public static ResultState Clip( Polygon poly, Line2D line, out List<Polygon> clippedPolygons ) {
			states.Clear();

			// first, figure out which side all points are on
			bool hasDiscards = false;
			int startIndex = -1;
			for( int i = 0; i < poly.Count; i++ ) {
				float sd = line.SignedDistance( poly[i] );
				if( Mathfs.Approximately( sd, 0 ) )
					states.Add( PointSideState.Edge );
				else if( sd > 0 ) {
					if( startIndex == -1 )
						startIndex = i;
					states.Add( PointSideState.Keep );
				} else {
					hasDiscards = true;
					states.Add( PointSideState.Discard );
				}
			}

			if( hasDiscards == false ) {
				clippedPolygons = null;
				return ResultState.OriginalLeftIntact;
			}

			if( startIndex == -1 ) {
				clippedPolygons = null;
				return ResultState.FullyDiscarded;
			}

			// find keep points, spread outwards until it's cut off from the rest
			SortedSet<PolygonSection> sections = new SortedSet<PolygonSection>();
			for( int i = 0; i < poly.Count; i++ ) {
				if( states[i] == PointSideState.Keep ) {
					sections.Add( ExtractPolygonSection( poly, line, i ) );
				}
			}

			// combine all clipped polygonal regions
			clippedPolygons = new List<Polygon>();
			while( sections.Count > 0 ) {
				// find solid polygon
				PolygonSection solid = sections.First();
				sections.Remove( solid );
				int solidDir = solid.tRange.Direction;

				// find holes in that polygon
				float referencePoint = solid.tRange.Min;
				while( true ) { // should break early anyway
					FloatRange checkRange = new FloatRange( referencePoint, solid.tRange.Max );
					PolygonSection hole = sections.FirstOrDefault( s => s.tRange.Direction != solidDir && checkRange.Contains( s.tRange ) );
					if( hole == null ) {
						// nothing inside - we're done with this solid
						clippedPolygons.Add( new Polygon( solid.points ) );
						break;
					} else {
						// append the hole polygon to the solid points
						sections.Remove( hole );
						if( solidDir == 1 )
							solid.points.InsertRange( 0, hole.points );
						else
							solid.points.AddRange( hole.points );

						referencePoint = hole.tRange.Max; // skip everything inside the hole by shifting forward
					}
				}
			}

			return ResultState.Clipped;
		}

		static PolygonSection ExtractPolygonSection( Polygon poly, Line2D line, int sourceIndex ) {
			List<Vector2> points = new List<Vector2>();

			void AddBack( int i ) {
				states[i.Mod( states.Count )] = PointSideState.Handled;
				points.Insert( 0, poly[i] );
			}

			void AddFront( int i ) {
				states[i.Mod( states.Count )] = PointSideState.Handled;
				points.Add( poly[i] );
			}
			
			AddFront( sourceIndex );

			float tStart = 0, tEnd = 0;
			for( int dir = -1; dir <= 1; dir += 2 ) {
				for( int i = 1; i < poly.Count; i++ ) {
					int index = sourceIndex + dir * i;
					if( states[index.Mod( states.Count )] is PointSideState.Discard or PointSideState.Edge ) {
						// hit the front edge
						Line2D edge = new Line2D( poly[index - dir], poly[index] - poly[index - dir] );
						if( IntersectionTest.LinearTValues( line, edge, out float tLine, out float tEdge ) ) {
							Vector2 intPt = edge.GetPoint( tEdge );
							if( dir == 1 ) {
								tEnd = tLine;
								points.Add( intPt );
							} else {
								tStart = tLine;
								points.Insert( 0, intPt );
							}

							break;
						}

						throw new Exception( "Polygon clipping failed due to line intersection not working as expected. You may have duplicate points or a degenerate polygon in general" );
					}

					// haven't hit the end yet, add current point
					if( dir == 1 )
						AddFront( index );
					else
						AddBack( index );
				}
			}

			return new PolygonSection( ( tStart, tEnd ), points );
		}


	}

}