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
    public partial class LispFunctions
    {
        // this file is intended to house utility functions that can be reused by the other functions

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
        
    }
}
