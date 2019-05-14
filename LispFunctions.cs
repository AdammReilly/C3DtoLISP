using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace C3DtoLISP
{
    public class LispFunctions
    {
        // check if the result buffer is not null, and a single input value of an ObjectId
        private bool BufferContainsObjectId(ResultBuffer resbuf)
        {
            // if the resbuf is empty, return nil
            if (resbuf == null)
                return false;
            // check if the resbuf is more than one argument
            var args = resbuf.AsArray();
            if (args.Length != 1)
                return false;
            // verify the resbuf is an objectId
            if (args[0].TypeCode != (int)LispDataType.ObjectId)
                return false;
            return true;
        }

        //creates a LISP compatable list of ObjectIds from an ObjectIdCollection
        public ResultBuffer ListObjectIds(ObjectIdCollection objectIds)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            try
            {
                using (Transaction ts = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                {
                    // iterate thru the object ids and create a list to return to LISP
                    ResultBuffer returnValue = new ResultBuffer();
                    if (objectIds.Count == 0)
                        return new ResultBuffer(nil);
                    returnValue.Add(new TypedValue((int)LispDataType.ListBegin));
                    foreach (ObjectId objectId in objectIds)
                    {
                        var item = (new TypedValue((int)LispDataType.ObjectId, objectId));
                        returnValue.Add(item);
                    }
                    returnValue.Add(new TypedValue((int)LispDataType.ListEnd));
                    return returnValue;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                // any error will dump a message to the console and return nil
                Console.WriteLine(ex.Message);
                return new ResultBuffer(nil);
            }
            // if something else happens, return nil
            return new ResultBuffer(nil);
        }


        #region LispFunctions
        // Checks if an object is a data reference
        // resbuf should be one objectId
        [LispFunction("IsReference")]
        public TypedValue IsDataReference(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            var T = new TypedValue((int)LispDataType.T_atom);
            if (BufferContainsObjectId(resbuf) == false)
                return nil;
            // get the actual value of the resbuf
            var args = resbuf.AsArray();
            ObjectId oID = (ObjectId)args[0].Value;

            Document currentDocument = Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;

            using (Transaction trans = currentDatabase.TransactionManager.StartTransaction())
            {
                // check if the arg is a civil 3d object
                try
                {
                    Autodesk.Civil.DatabaseServices.Entity selectedEntity = (Autodesk.Civil.DatabaseServices.Entity)trans.GetObject(oID, OpenMode.ForRead);
                    // check if the civil object is a data reference
                    if (selectedEntity.IsReferenceObject)
                    { return T; }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return nil;
                }
            }
            // return nil as the final output option
            return nil;
        }

        // synchronize a data reference
        // there's no api call for synchonizing, this may be a bust command
        [LispFunction("Synchronize")]
        public TypedValue SyncDataReference(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            var T = new TypedValue((int)LispDataType.T_atom);
            // check if the arg is a civil 3d object
            if (BufferContainsObjectId(resbuf) == false)
                return nil;
            // get the actual value of the resbuf
            var args = resbuf.AsArray();
            ObjectId oID = (ObjectId)args[0].Value;

            Document currentDocument = Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;

            using (Transaction trans = currentDatabase.TransactionManager.StartTransaction())
            {
                try
                {
                    Autodesk.Civil.DatabaseServices.Entity selectedEntity = (Autodesk.Civil.DatabaseServices.Entity)trans.GetObject(oID, OpenMode.ForRead);
                    // check if the civil object is a data reference
                    if (selectedEntity.IsReferenceObject)
                    {
                        // synchronize the entity
                        // no code entry point available to synchronize an individual object
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return nil;
                }
            }
            
            // return nil as the final output option
            return nil;
        }

        // rebuild a surface object
        [LispFunction("RebuildSurface")]
        public TypedValue RebuildSurface(ResultBuffer resbuf)
        {
            var nil = new TypedValue((int)LispDataType.Nil);
            var T = new TypedValue((int)LispDataType.T_atom);
            if (BufferContainsObjectId(resbuf) == false)
                return nil;
            // get the actual value of the resbuf
            var args = resbuf.AsArray();
            ObjectId oID = (ObjectId)args[0].Value;

            Document currentDocument = Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;

            using (Transaction trans = currentDatabase.TransactionManager.StartTransaction())
            {
                try
                {
                    //check if the oID belongs to a surface
                    Autodesk.Civil.DatabaseServices.Surface surface = oID.GetObject(OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.Surface;
                    // make sure it's not a reference object
                    // check if the civil object is a data reference
                    if (surface != null)
                    {
                        if (surface.IsReferenceObject == false)
                        {
                            // rebuild the object
                            surface.Rebuild();
                            return T;
                        }
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return nil;
                }
            }

            // return nil as the final output option
            return nil;
        }

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

        #endregion
        

    }
}
