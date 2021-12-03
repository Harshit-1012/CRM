using CallingCRM.Helpers;
using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class TemplateServices
    {
        public hwLiveEntities _context = new hwLiveEntities();
        public CommonHelper _helper = new CommonHelper();

        public List<EmailTemplate> GetTemplateListByUser(string Id,string role)
        {
            List<EmailTemplate> result;
            if (role == "Agent")
            {
                result = _context.EmailTemplates.Where(x => x.AdminId == Id && x.IsActive== true && x.AllowedRole == "Telecaller" && x.IsDeleted == false).ToList();
                return result;
            }
            if(role == "Manager")
            {
                result = _context.EmailTemplates.Where(x => x.AdminId == Id && x.IsActive == true && x.IsDeleted == false).ToList();
                return result;
            }

            result = _context.EmailTemplates.Where(x => x.AdminId == Id && x.IsDeleted == false).ToList();
            return result;
        }

        public EmailTemplate GetTemplateById(string id, string UserId)
        {
            int? ID = Convert.ToInt32(id);
            EmailTemplate Template = _context.EmailTemplates.Where(x => x.Id == ID && x.AdminId == UserId && x.IsDeleted==false).FirstOrDefault();
            return Template;
        }


        public void UpdateTemplate(EmailTemplate Template)
        {
            
            EmailTemplate template = _context.EmailTemplates.Where(x => x.Id ==Template.Id).FirstOrDefault();

            if (template != null)
            {
                template.AllowedRole = Template.AllowedRole;
                template.EmailSubject = Template.EmailSubject;
                template.EmailBody = Template.EmailBody;
                template.IsActive = Template.IsActive;
                template.TemplateName = Template.TemplateName;
                
            }
            else
            {
                EmailTemplate newtemplate = new EmailTemplate();
                newtemplate.TemplateName = Template.TemplateName;
                newtemplate.AllowedRole = Template.AllowedRole;
                newtemplate.EmailSubject = Template.EmailSubject;
                newtemplate.EmailBody = Template.EmailBody;
                newtemplate.IsActive = Template.IsActive;
                newtemplate.IsDeleted = false;
                newtemplate.CreatedDate = _helper.GetCurrentDate();
                newtemplate.AdminId = ConfigurationManager.AppSettings["ClientId"].ToString();
                _context.EmailTemplates.Add(newtemplate);
            }
            _context.SaveChanges();

        }

        public void deleteTemplate(long? id)
        {
            EmailTemplate template = _context.EmailTemplates.Where(x => x.Id == id).FirstOrDefault();
            template.IsDeleted = true;
            _context.SaveChanges();
        }

    }

}