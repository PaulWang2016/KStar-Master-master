
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    /// 流程配置的用户
    /// </summary>
    public class Process_ControlSettingDTO
    {
        public Guid SysId { get; set; }
        public int ActivityId { get; set; }
        public ProcessWorkMode WorkMode { get; set; }
        public string ControlRenderId { get; set; }
        public string ControlName { get; set; }
        public ProcessControlType ControlType { get; set; }
        public bool IsHide { get; set; }
        public bool IsDisable { get; set; }
        public bool IsCustom { get; set; }
        public string RenderTemplateId { get; set; }
        public string ProcessFullName { get; set; }
        public string RenderTemplateName { get; set; }
        public string ComponentId { get; set; }
    }

    public enum ProcessControlType
    {
        Control,
        Button,
        Component
    }

    public enum ProcessWorkMode
    {
        View,
        Draft,
        Startup,
        Approval,
        Review
    }
}
