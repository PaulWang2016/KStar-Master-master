using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using aZaaS.KStar.Report;

namespace aZaaS.KStar.Web.Models
{
    public class ReportItemModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; }
        public String Department { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public string ReportCode { get; set; }
        public string Status { get; set; }
        /// <summary>
        /// 点击率
        /// </summary>
        public string Rate { get; set; }
        public string ReportUrl { get; set; }
        public string ImageThumbPath { get; set; }
        public string Comment { get; set; }
        public Guid ParnentID { get; set; }
        //public string PermissionRoleNames { get; set; }
        //public string PermissionRoleIds { get; set; }

        public string Roles { get; set; }
        public string FormattedRoles { get; set; }

        public ReportInfoEntity ToEntity()
        {
            var entity = new ReportInfoEntity()
            {
                ID = this.ID,
                ParnentID = this.ParnentID,
                Name = this.Name ?? "",
                Department = this.Department ?? "",
                PublishedDate = this.PublishedDate,
                Level = this.Level ?? "",
                Category = this.Category ?? "",
                ReportCode = this.ReportCode ?? "",
                Status = this.Status ?? "",
                Rate = this.Rate ?? "0",
                ReportUrl = this.ReportUrl,
                ImageThumbPath = this.ImageThumbPath ?? "",
                Comment = this.Comment ?? ""
            };

            return entity;
        }


        public void FromReport(ReportInfoEntity report, List<ReportPermission> permissions)
        {
            ID = report.ID;
            ParnentID = report.ParnentID;
            Name = report.Name;
            Department = report.Department;
            PublishedDate = report.PublishedDate;
            Level = report.Level;
            Category = report.Category;
            ReportCode = report.ReportCode;
            Status = report.Status;
            Rate = report.Rate;
            ReportUrl = report.ReportUrl;
            ImageThumbPath = report.ImageThumbPath;
            Comment = report.Comment;

            if (permissions != null && permissions.Any())
            {
                Roles = JsonConvert.SerializeObject(permissions);
                FormattedRoles = string.Join(",", permissions.Select(p => p.RoleName).ToArray());
            }
        }

        public List<ReportPermission> ResolvePermissions()
        {
            var permissions = new List<ReportPermission>();

            if (string.IsNullOrEmpty(this.Roles))
                return permissions;

            var roleStrs = this.Roles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (roleStrs.Length <= 0)
                return permissions;

            foreach (var item in roleStrs)
            {
                var fields = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                permissions.Add(new ReportPermission()
                {
                    ID = Guid.NewGuid(),
                    ReportID = this.ID,
                    RoleID = Guid.Parse(fields[0]),
                    RoleName = fields[2],
                    RoleType = fields[1]
                });

            }


            return permissions;
        }
    }

    internal static partial class Extensions
    {
        public static void FromData(this ReportInfoEntity entity, ReportItemModel report)
        {
            //entity.ID = report.ID;
            entity.Name = report.Name ?? "";
            entity.Category = report.Category ?? "";
            entity.ParnentID = report.ParnentID;
            entity.Level = report.Level ?? "";
            entity.Rate = report.Rate ?? "0";
            entity.ReportCode = report.ReportCode ?? "";
            entity.ReportUrl = report.ReportUrl ?? "";
            entity.Comment = report.Comment ?? "";
            entity.Department = report.Department ?? "";
            entity.Status = report.Status ?? "";
        }
    }
}