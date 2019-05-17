using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using System;


namespace C3DtoLISP
{
    public partial class LispFunctions
    {
        // This region corresponds to the Autodesk.Civil.ApplicationServices.CivilDocument class
        #region CivilDocument
        // get the ObjectID collection of all surfaces in the drawing
        [LispFunction("GetSurfaceIds")]
        public ResultBuffer GetSurfaceIds(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            if (resbuf == null)
            {
                CivilDocument currentDocument = CivilApplication.ActiveDocument;
                ObjectIdCollection SurfaceIds = currentDocument.GetSurfaceIds();
                ResultBuffer returnValue = ListObjectIds(SurfaceIds);
                return returnValue;
            }
            return new ResultBuffer(nil);
        }

        // get the ObjectID collection of all alignments in the drawing
        [LispFunction("GetAlignmentIds")]
        public ResultBuffer GetAlignmentIds(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            if (resbuf == null)
            {
                CivilDocument currentDocument = CivilApplication.ActiveDocument;
                ObjectIdCollection alignmentIds = currentDocument.GetAlignmentIds();
                ResultBuffer returnValue = ListObjectIds(alignmentIds);
                return returnValue;
            }
            return new ResultBuffer(nil);
        }

        // get the ObjectID collection of all pipe networks in the drawing
        [LispFunction("GetPipeNetworkIds")]
        public ResultBuffer GetPipeNetworkIds(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            if (resbuf == null)
            {
                CivilDocument currentDocument = CivilApplication.ActiveDocument;
                ObjectIdCollection pipeNetworkIds = currentDocument.GetPipeNetworkIds();
                ResultBuffer returnValue = ListObjectIds(pipeNetworkIds);
                return returnValue;
            }
            return new ResultBuffer(nil);
        }

        // get the ObjectID collection of all the points in the drawing
        [LispFunction("GetAllPointIds")]
        public ResultBuffer GetAllPointIds(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            if (resbuf == null)
            {
                CivilDocument currentDocument = CivilApplication.ActiveDocument;
                ObjectIdCollection allPointIds = currentDocument.GetAllPointIds();
                ResultBuffer returnValue = ListObjectIds(allPointIds);
                return returnValue;
            }
            return new ResultBuffer(nil);
        }

        // get the ObjectID collection of all corridors in the drawing
        [LispFunction("GetCorridorIds")]
        public ResultBuffer GetCorridorIds(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            if (resbuf == null)
            {
                CivilDocument currentDocument = CivilApplication.ActiveDocument;
                Autodesk.Civil.DatabaseServices.CorridorCollection corridorColl = currentDocument.CorridorCollection;
                ObjectIdCollection corridorIds = new ObjectIdCollection();
                foreach (Autodesk.Civil.DatabaseServices.Corridor corr in corridorIds)
                {
                    corridorIds.Add(corr.ObjectId);
                }
                ResultBuffer returnValue = ListObjectIds(corridorIds);
                return returnValue;
            }
            return new ResultBuffer(nil);
        }

        //
        //TO DO: Add the following functions:
        // GetAlignmentTableIds
        // GetCivilDocument
        // GetGeneralSegmentLabelIds
        // GetIntersectionIds
        // GetLegendTableIds
        // GetNoteLabelIds
        // GetParcelSegmentTableIds
        // GetParcelTableIds
        // GetPointTableIds
        // GetSiteIds
        // GetSitelessAlignmentId
        // GetSitelessAlignmentIds
        // GetViewGrameGroupIds
        // AssemblyCollection
        // CogoPoints
        // CorridorState
        // IsCorridorSectionViewActive
        // IsDriveActive
        // IsSectoinEditorCorridorReferenceObject
        // NetworkState
        // PointGroups
        // PointUDPClassifications
        // PointUDPs
        // Settings
        // Styles
        // SubassemblyCollection
        //

        #endregion

        // This region corresponds to the Autodesk.Civil.ApplicationServices.CivilApplication class
        #region CivilApplication

        //
        // TO DO: Add:
        // ActiveDocument
        // ActiveProduct
        // SurveyProjects
        //


        #endregion

        #region ProductType
            // should return the string value of the number provided as a product type
        [LispFunction("ProductType")]
        public ResultBuffer ProductType(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            if (resbuf == null)
            {
                var args = resbuf.AsArray();
                if (args.Length == 1)
                {
                   ResultBuffer returnValue = new ResultBuffer
                        {
                        Autodesk.Civil.ApplicationServices.ProductType.GetName(typeof(Autodesk.Civil.ApplicationServices.ProductType), 0)
                        };
                   return returnValue;
                }
            }
            return new ResultBuffer(nil);
        }


        #endregion

    }
}
