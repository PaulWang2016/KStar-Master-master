using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form
{
    public class DataError
    {
        public AlertType AlertType { get; set; }
        public string Message { get; set; }

        public DataError(AlertType type, string errorMessage)
        {
            AlertType = type;
            Message = errorMessage;
        }
    }

    public enum AlertType
    {
        Warning,
        Error
    }
}
