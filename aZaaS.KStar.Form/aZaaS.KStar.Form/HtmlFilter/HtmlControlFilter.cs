using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aZaaS.KStar.Form.Infrastructure;
using CsQuery;

namespace aZaaS.KStar.Form.HtmlFilter
{
    public sealed class HtmlControlFilter
    {
        private CQ _doc;
        private WorkMode _workMode;
        private string _processName;
        private readonly IFormSettingProvider _formSettingProvider;
        private readonly IControlTmplProvider _controlTmplProvider;

        public HtmlControlFilter(string html)
        {
            _doc = CQ.CreateDocument(html);
            _formSettingProvider = new KStarFormSettingProvider();
            _controlTmplProvider = new ControlTmplProvider();
        }

        public HtmlControlFilter(string html, WorkMode workMode, string processName)
        {
            _doc = CQ.CreateDocument(html);
            _workMode = workMode;
            _processName = processName;
            _formSettingProvider = new KStarFormSettingProvider();
            _controlTmplProvider = new ControlTmplProvider();
        }

        public void Clear(string containerSelector)
        {
            var selection = _doc.Select(containerSelector);

            if (selection != null)
                selection.Html(string.Empty);
        }

        public void Remove(string selector)
        {
            var selection = _doc.Select(selector);

            if (selection != null)
                selection.Remove();
        }

        public void Replace(string selector, CQ replacer)
        {
            var selection = _doc.Select(selector);

            if (selection != null && replacer != null)
                selection.ReplaceWith(replacer);
        }

        public void Html(string selector, CQ replacer)
        {
            var selection = _doc.Select(selector);

            if (selection != null && replacer != null)
                selection.Html(replacer.Render());
        }

        public void Disable(string selector)
        {
            var selection = _doc.Select(selector);

            if (selection != null)
            {
                selection.Prop("disabled", true);
                //a标签不禁用
                selection.Find("*").Not("a").Prop("disabled", true);
            }
        }

        public void Filter(int activityId)
        {
            var selector = string.Empty;
            switch (_workMode)
            {
                case WorkMode.View:
                case WorkMode.Review:
                    DisableByViewMode();
                    break;
                case WorkMode.Approval:
                    DisableByApproval();
                    break;
            }

            var controls = _formSettingProvider.GetControlSettings(_processName, activityId, _workMode).ToList();

            var CustomControls = controls.FindAll(control => control.IsHide == false && control.IsCustom == true).ToList();
            var DisableControls = controls.FindAll(control => control.IsHide == false && control.IsDisable == true).ToList();
            var HideControls = controls.FindAll(control => control.IsHide == true).ToList();

            CustomControls.ForEach(item =>
            {
                var replacer = GetReplacer(item.RenderTemplateId);

                selector = GetSelector(item.ControlRenderId, item.ControlType);

                if (item.ControlType == ControlType.Component)
                {
                    Html(selector, replacer);
                }
                else
                {
                    Replace(selector, replacer);
                }
            });
            DisableControls.ForEach(item =>
            {
                selector = GetSelector(item.ControlRenderId, item.ControlType);

                Disable(selector);
            });
            HideControls.ForEach(item =>
            {
                selector = GetSelector(item.ControlRenderId, item.ControlType);

                Remove(selector);
            });
        }

        public void Filter(int activityId, IEnumerable<string> showIdList)
        {
            var selector = string.Empty;
            switch (_workMode)
            {
                case WorkMode.View:
                case WorkMode.Review:
                    DisableByViewMode();
                    break;
                case WorkMode.Approval:
                    DisableByApproval();
                    break;
            }

            var controls = _formSettingProvider.GetControlSettings(_processName, activityId, _workMode).ToList();

            var CustomControls = controls.FindAll(control => control.IsHide == false && control.IsCustom == true).ToList();
            var DisableControls = controls.FindAll(control => control.IsHide == false && control.IsDisable == true).ToList();
            var HideControls = controls.FindAll(control => control.IsHide == true).ToList();

            CustomControls.ForEach(item =>
            {
                var replacer = GetReplacer(item.RenderTemplateId);

                selector = GetSelector(item.ControlRenderId, item.ControlType);

                if (item.ControlType == ControlType.Component)
                {
                    Html(selector, replacer);
                }
                else
                {
                    Replace(selector, replacer);
                }
            });
            DisableControls.ForEach(item =>
            {
                selector = GetSelector(item.ControlRenderId, item.ControlType);

                Disable(selector);
            });
            HideControls.ForEach(item =>
            {
                selector = GetSelector(item.ControlRenderId, item.ControlType);

                if (!showIdList.Contains(item.ControlRenderId))
                {
                    Remove(selector);
                }
                else
                {
                    Disable(selector);
                }
            });
        }

        public string Render()
        {
            return _doc.Render();
        }

        public IEnumerable<string> GetShowIdList()
        {
            var idList = new HashSet<string>();
            var kstar_component_list = _doc[".kstar-component"].ToList();
            //var kstar_button_list = _doc[".kstar-button"].ToList();
            var kstar_control_list = _doc[".kstar-control"].ToList();
            foreach (var control in kstar_component_list)
            {
                idList.Add(control.Id);
            }
            //foreach (var control in kstar_button_list)
            //{
            //    idList.Add(control.Id);
            //}
            foreach (var control in kstar_control_list)
            {
                idList.Add(control.Id);
            }

            return idList;
        }

        private string GetSelector(string controlId, ControlType controlType)
        {
            var selector = string.Empty;

            if (controlType == ControlType.Component)
            {
                selector = string.Format("div[role={0}]", controlId);
            }
            else
            {
                selector = string.Format("#{0}", controlId);
            }

            return selector;
        }

        private CQ GetReplacer(string templateId)
        {
            Guid guid;
            Guid.TryParse(templateId, out guid);
            var template = _controlTmplProvider.GetTemplate(guid);

            if (template == null)
            {
                return null;
            }

            CQ replacer = CQ.Create(template.HtmlTemplate);
            return replacer;
        }

        private void DisableByViewMode(string selector)
        {
            var selection = _doc.Select(selector);

            if (selection != null)
            {
                selection.Prop("disabled", true);
                selection.Find("*").Not("a").Prop("disabled", true);
            }
        }

        private void DisableByViewMode()
        {
            var kstarform_header = "_kstarform_header";
            var kstarform_content = "_kstarform_content";
            var kstarform_attachment = "_kstarform_attachment";

            var header = string.Format("div[role={0}]", kstarform_header);
            var content = string.Format("div[role={0}]", kstarform_content);
            var attachment = string.Format("div[role={0}]", kstarform_attachment);

            DisableByViewMode(header);
            DisableByViewMode(content);
            DisableByViewMode(attachment);
        }

        private void DisableByApproval()
        {
            var kstarform_header = "_kstarform_header";

            var header = string.Format("div[role={0}]", kstarform_header);

            Disable(header);
        }
    }
}
