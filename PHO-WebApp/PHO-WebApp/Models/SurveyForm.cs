using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class SurveySummary
    {
        [Key]
        public int FormId { get; set; }
        public string Survey_Title { get; set; }
        public string Description { get; set; }
    }

    public class SurveyForm
    {
        public int FormId { get; set; }
        public string Survey_Title { get; set; }

        public List<FormSection> FormSections = new List<FormSection>();
        public FormSection LastFormSection
        {
            get
            {
                if (this.FormSections != null && this.FormSections.Count > 0)
                {
                    return FormSections[FormSections.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        
    }
    public class FormSection
    {
        public int FormSectionId { get; set; }
        public int Order { get; set; }
        public List<Section> Sections = new List<Section>();
        public Section LastSection
        {
            get
            {
                if (this.Sections != null && this.Sections.Count > 0)
                {
                    return Sections[Sections.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class Section
    {
        public int SectionId { get; set; }
        public string SectionDescription { get; set; }
        public List<SectionQuestion> SectionQuestions = new List<SectionQuestion>();
        public SectionQuestion LastSectionQuestion
        {
            get
            {
                if (this.SectionQuestions != null && this.SectionQuestions.Count > 0)
                {
                    return SectionQuestions[SectionQuestions.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class SectionQuestion
    {
        public int SectionQuestionId { get; set; }
        public int Order { get; set; }
        public List<Question> Questions = new List<Question>();
        public Question LastQuestion
        {
            get
            {
                if (this.Questions != null && this.Questions.Count > 0)
                {
                    return Questions[Questions.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionType { get; set; }
        public int Order { get; set; }
        public string Flag_Required { get; set; }
        public string QuestionLabel { get; set; }
        public string LabelCode { get; set; }
        public string Javascript { get; set; }

        public List<QuestionAnswerOption> QuestionAnswerOptions = new List<QuestionAnswerOption>();
    }

    public class QuestionAnswerOption
    {
        public int QuestionAnswerOptionId { get; set; }
        public int AnswerOptionId { get; set; }
        public string QuestionAnswerOptionLabel { get; set; }
    }


}