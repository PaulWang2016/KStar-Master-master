using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Repositories
{
    public class FlowThemeRepository
    {
        public ProcessFormFlowTheme Get(string processFullName)
        {

            using (var db = new aZaaSKStarFormContext())
            {
                ProcessFormFlowTheme fileModel = null;
                fileModel = db.ProcessFormFlowThemes.FirstOrDefault<ProcessFormFlowTheme>(x => x.ProcessFullName == processFullName);
                return fileModel;
            }
        }

        public List<ProcessFormFlowTheme> GetAll()
        {
            var files = new List<ProcessFormFlowTheme>();
            using (var db = new aZaaSKStarFormContext())
            {
                var ProcessFormFlowTheme = db.ProcessFormFlowThemes.ToList();

                ProcessFormFlowTheme.ForEach(item =>
                {
                    var fileModel = new ProcessFormFlowTheme()
                    {
                       ID = item.ID,
                       ProcessFullName = item.ProcessFullName, 
                       ModelFullName=item.ModelFullName,
                       RuleString = item.RuleString,
                       Name = item.Name  
                    }; 
                    files.Add(fileModel);
                });
            } 
            return files;
        }

        public bool Add(ProcessFormFlowTheme flowTheme)
        {
            if (flowTheme == null || string.IsNullOrWhiteSpace(flowTheme.ModelFullName) || string.IsNullOrWhiteSpace(flowTheme.Name) || string.IsNullOrWhiteSpace(flowTheme.ProcessFullName) || string.IsNullOrWhiteSpace(flowTheme.RuleString))
            {
                throw new Exception("Null Reference");
            } 
            using (var db = new aZaaSKStarFormContext())
            {
                flowTheme.ID = 0;
                db.ProcessFormFlowThemes.Add(flowTheme);
                db.SaveChanges();
                return true;
            } 
        }

        public bool Delete(int id)
        {
            using (var db = new aZaaSKStarFormContext())
            {
                ProcessFormFlowTheme entity = db.ProcessFormFlowThemes.FirstOrDefault(x => x.ID == id);
                db.ProcessFormFlowThemes.Remove(entity);
                db.SaveChanges();
                return true;
            }
        }
    }
}
