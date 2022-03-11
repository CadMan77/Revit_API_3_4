//Создать приложение, которое для всех труб в модели записывает значение в созданный (заблаговременно,
//вручную общий, текстовый, связанный с категорией Pipes) параметр («Наименование») в следующую формате 
//«Труба НАРУЖНЫЙ_ДИАМЕТР / ВНУТРЕННИЙ_ДИАМЕТР», где НАРУЖНЫЙ_ДИАМЕТР и ВНУТРЕННИЙ_ДИАМЕТР
//соответствующие диаметры трубы в миллиметрах.

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit_API_3_4
{
    [Transaction(TransactionMode.Manual)]

    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
            int pipeQty = 0;

            //int pipeQty2 = new FilteredElementCollector(doc)
            //    .OfClass(typeof(Pipe))
            //    .GetElementCount();

                FilteredElementCollector pipeCollector = new FilteredElementCollector(doc);
                pipeCollector.OfCategory(BuiltInCategory.OST_PipeCurves);

                foreach (Element elem in pipeCollector)
                {
                    if (!(elem is PipeType))
                    //if (elem is Pipe)
                    {                    
                        pipeQty += 1;

                        //Parameter outDia = elem.LookupParameter("Outside Diameter");
                        //Parameter inDia = elem.LookupParameter("Inside Diameter");

                        Parameter outDia = elem.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                        Parameter inDia = elem.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);



                        //TaskDialog.Show($"Pipe#{pipeQty}", $"Outside Dia - {outDia.AsValueString()}{Environment.NewLine}Inside Dia - {inDia.AsValueString()}");

                        Double outDiaMM = UnitUtils.ConvertFromInternalUnits(outDia.AsDouble(), UnitTypeId.Millimeters);
                        Double inDiaMM = UnitUtils.ConvertFromInternalUnits(inDia.AsDouble(), UnitTypeId.Millimeters);

                        using (Transaction ts = new Transaction(doc, "Set Shared Parameter Value"))
                        {
                            ts.Start();
                            Parameter naimenovanieParameter = elem.LookupParameter("Наименование");

                            //naimenovanieParameter.Set($"Труба {outDia.AsValueString()}/{inDia.AsValueString()}");
                            naimenovanieParameter.Set($"Труба {outDiaMM}/{inDiaMM}");
                            ts.Commit();
                        }

                        //TaskDialog.Show($"Pipe#{pipeQty}", $"Наименование - \"{elem.LookupParameter("Наименование").AsValueString()}\"");                        
                    }
                }

                TaskDialog.Show("Завершено", $"Значение свойства \"Наименование\" скорректировано у {pipeQty} труб."); // {Environment.NewLine}{pipeQty2}");
            }
            catch { }
            return Result.Succeeded;
        }
    }
}
