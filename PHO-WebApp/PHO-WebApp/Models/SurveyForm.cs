using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Mvc;

namespace PHO_WebApp.Models
{
    public enum QuestionTypeEnum
    {
        TextBox = 1,
        CheckBox = 2,
        CheckBoxList = 3,
        RadioButtonList = 4,
        DropDownList = 5,
        TextBoxNumeric = 6,
        Calculated = 7
    }

    public class SurveySummary
    {
        private string _description;
        private string _survey_Title;
        private int _formId;
        private int _totalResponses;
        private int _completedResponses;
        private int _inprogressResponses;
        private List<SurveyFormResponse> _recentInProgress;

        [Key]
        public int FormId
        {
            get { return _formId; }
            set { _formId = value; }
        }
        public string Survey_Title
        {
            get { return _survey_Title; }
            set { _survey_Title = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int TotalResponses
        {
            get { return _totalResponses; }
            set { _totalResponses = value; }
        }

        public int CompletedResponses
        {
            get { return _completedResponses; }
            set { _completedResponses = value; }
        }

        public int InProgressResponses
        {
            get { return _inprogressResponses; }
            set { _inprogressResponses = value; }
        }

        public List<SurveyFormResponse> RecentInProgress
        {
            get { return _recentInProgress; }
            set { _recentInProgress = value; }
        }
    }

    public class SurveyForm
    {
        private List<FormSection> _FormSections; 
        private string _survey_Title;
        private int _formId;
        private int _formResponseId;

        public int FormId
        {
            get { return _formId; }
            set { _formId = value; }
        }
        
        public int FormResponseId
        {
            get { return _formResponseId; }
            set { _formResponseId = value; }
        }

        public string Survey_Title
        {
            get { return _survey_Title; }
            set { _survey_Title = value; }
        }

        public List<FormSection> FormSections
        {
            get
            {
                if (this._FormSections == null)
                {
                    this._FormSections = new List<FormSection>();
                }
                return this._FormSections;
            }
            set
            {
                this._FormSections = value;
            }
        }
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

        public int PercentComplete
        {
            get
            {
                int total = 0;
                int complete = 0;
                foreach(QuestionResponse resp in this.Responses)
                {
                    total++;
                    if (!string.IsNullOrWhiteSpace(resp.Response_Text) || (resp.ResponseAnswer != null && resp.ResponseAnswer.AnswerOptionId > 0))
                    {
                        complete++;
                    }
                }
                return (int)Math.Round(((double)complete / total) * 100, 0);
            }
        }

        public List<QuestionResponse> Responses
        {
            get
            {
                List<QuestionResponse> responses = new List<QuestionResponse>();

                if (this.FormSections != null && this.FormSections.Count > 0)
                {
                    foreach (FormSection fs in this.FormSections)
                    {
                        if (fs.Sections != null && fs.Sections.Count > 0)
                        {
                            foreach(Section s in fs.Sections)
                            {
                                if (s.SectionQuestions != null && s.SectionQuestions.Count > 0)
                                {
                                    foreach(SectionQuestion sq in s.SectionQuestions)
                                    {
                                        if (sq.Question != null)
                                        {
                                            responses.Add(sq.Question);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return responses;
            }
        }


    }
    public class FormSection
    {
        private List<Section> _Sections;
        private int _order;
        private int _formSectionId;

        public int FormSectionId
        {
            get { return _formSectionId; }
            set { _formSectionId = value; }
        }
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }
        public List<Section> Sections
        {
            get
            {
                if (this._Sections == null)
                {
                    this._Sections = new List<Section>();
                }
                return this._Sections;
            }
            set
            {
                this._Sections = value;
            }
        }
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
        private List<SectionQuestion> _SectionQuestions;
        private int _sectionId;
        private string _sectionDescription;

        public int SectionId
        {
            get { return _sectionId; }
            set { _sectionId = value; }
        }
        public string SectionDescription
        {
            get { return _sectionDescription; }
            set { _sectionDescription = value; }
        }
        public List<SectionQuestion> SectionQuestions
        {
            get
            {
                if (this._SectionQuestions == null)
                {
                    this._SectionQuestions = new List<SectionQuestion>();
                }
                return this._SectionQuestions;
            }
            set
            {
                this._SectionQuestions = value;
            }
        }
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
        public QuestionResponse _Question;
        private int _sectionQuestionId;
        private int _order;

        public int SectionQuestionId
        {
            get { return _sectionQuestionId; }
            set { _sectionQuestionId = value; }
        }
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }
        //public List<Question> Questions = new List<Question>();
        public QuestionResponse Question
        {
            get
            {
                if (this._Question == null)
                {
                    this._Question = new QuestionResponse();
                }
                return this._Question;
            }
            set
            {
                this._Question = value;
            }
        }
    }

    public class SurveyFormResponse
    {
        private int _formResponseId;
        private int _formId;
        private DateTime _dateCreated;
        private DateTime _dateModfied;
        private DateTime _dateCompleted;
        private bool _completed;
        private int _PercentCompleted;

        public int FormResponseId
        {
            get { return _formResponseId; }
            set { _formResponseId = value; }
        }

        public int FormId
        {
            get { return _formId; }
            set { _formId = value; }
        }

        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = value; }
        }

        public DateTime DateModified
        {
            get { return _dateModfied; }
            set { _dateModfied = value; }
        }

        public DateTime DateCompleted
        {
            get { return _dateCompleted; }
            set { _dateCompleted = value; }
        }

        public bool Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }
        public int PercentCompleted
        {
            get { return _PercentCompleted; }
            set { _PercentCompleted = value; }
        }
    }
    
    [FluentValidation.Attributes.Validator(typeof(ResponseValidator))]
    public class QuestionResponse
    {
        private string _javascript;
        private string _labelCode;
        private string _questionLabel;
        private bool _flag_Required;
        private int _order;
        private string _questionType;
        private int _questionTypeId;
        private int _questionId;
        private string _response_Text;
        private ResponseAnswer _responseAnswer;
        private List<QuestionAnswerOption> _questionAnswerOptions;

        //response
        private int _responseId;
        private int _formResponseId;
               

        public int QuestionId
        {
            get { return _questionId; }
            set { _questionId = value; }
        }
        public int QuestionTypeId
        {
            get { return _questionTypeId; }
            set { _questionTypeId = value; }
        }
        public int FormResponseId
        {
            get { return _formResponseId; }
            set { _formResponseId = value; }
        }
        public int ResponseId
        {
            get { return _responseId; }
            set { _responseId = value; }
        }
        public string QuestionType
        {
            get { return _questionType; }
            set { _questionType = value; }
        }
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }
        public bool Flag_Required
        {
            get { return _flag_Required; }
            set { _flag_Required = value; }
        }
        public string QuestionLabel
        {
            get { return _questionLabel; }
            set { _questionLabel = value; }
        }
        public string LabelCode
        {
            get { return _labelCode; }
            set { _labelCode = value; }
        }
        public string Javascript
        {
            get { return _javascript; }
            set { _javascript = value; }
        }

        public List<QuestionAnswerOption> QuestionAnswerOptions
        {
            get { return _questionAnswerOptions; }
            set { _questionAnswerOptions = value; }
        }

        public string Response_Text
        {
            get { return _response_Text; }
            set
            {
                _response_Text = value;
            }
        }

        public ResponseAnswer ResponseAnswer
        {
            get { return _responseAnswer; }
            set { _responseAnswer = value; }
        }

        public bool IsListQuestionType
        {
            get
            {
                bool returnValue = false;

                QuestionTypeEnum[] ListQuestionTypes = new QuestionTypeEnum[] { QuestionTypeEnum.CheckBoxList, QuestionTypeEnum.DropDownList, QuestionTypeEnum.RadioButtonList };

                if (this.QuestionTypeId > 0)
                {
                    foreach (QuestionTypeEnum type in ListQuestionTypes)
                    {
                        if (this.QuestionTypeId == (int)type)
                        {
                            returnValue = true;
                        }
                    }
                }

                return returnValue;
            }
        }
    }

    //[FluentValidation.Attributes.Validator(typeof(ResponseValidator))]
    //public class Response
    //{
    //    private int _QuestionId;
    //    private string _response_Text;
    //    private ResponseAnswer _responseAnswer;
    //    private bool _required;

    //    public int QuestionId
    //    {
    //        get { return _QuestionId; }
    //        set { _QuestionId = value; }
    //    }


    //    public bool Required
    //    {
    //        get { return _required; }
    //        set { _required = value; }
    //    }

    //}

    public class ResponseAnswer
    {
        private int _ResponseId;
        private int _AnswerOptionId;
        private int _ResponseAnswersId;

        public int ResponseId
        {
            get { return _ResponseId; }
            set { _ResponseId = value; }
        }
        public int AnswerOptionId
        {
            get { return _AnswerOptionId; }
            set { _AnswerOptionId = value; }
        }

        public int ResponseAnswersId
        {
            get { return _ResponseAnswersId; }
            set { _ResponseAnswersId = value; }
        }
    }





    public class QuestionAnswerOption
    {
        private int _questionId;
        private int _questionAnswerOptionId;
        private int _answerOptionId;
        private string _questionAnswerOptionLabel;

        public int QuestionAnswerOptionId
        {
            get { return _questionAnswerOptionId; }
            set { _questionAnswerOptionId = value; }
        }
        public int AnswerOptionId
        {
            get { return _answerOptionId; }
            set { _answerOptionId = value; }
        }
        public string QuestionAnswerOptionLabel
        {
            get { return _questionAnswerOptionLabel; }
            set { _questionAnswerOptionLabel = value; }
        }

        public int QuestionId
        {
            get { return _questionId; }
            set { _questionId = value; }
        }
    }

    public class ResponseValidator : AbstractValidator<QuestionResponse>
    {
        public ResponseValidator()
        {
            When(x => x.Flag_Required==true, () => {
                RuleFor(x => x.Response_Text).NotEmpty().WithMessage("Required");
            });

            When(x => (x.Flag_Required==true && x.IsListQuestionType == true), () =>
            {
                //RuleFor(x => x.ResponseAnswer).NotNull().WithMessage("Required");
                //RuleFor(x => x.ResponseAnswer.AnswerOptionId).NotNull().NotEmpty().WithMessage("Required");
            });
        }
    }

    
}

