using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;
namespace Webb.Playbook.Data
{
    public class Call : ISerializableObj
    {
        public Call()
        {
            Questions = new ObservableCollection<Question>();
           
            Answers = new ObservableCollection<Answer>();
      
        }

        private ObservableCollection<Question> qustions;
        public ObservableCollection<Question> Questions
        {
            get
            {
                if (qustions == null)
                {
                    qustions = new ObservableCollection<Question>();
                }
                return qustions;
            }
            set { qustions = value; }
        }
        private ObservableCollection<Answer> answers;
        public ObservableCollection<Answer> Answers
        {
            get
            {
                if (answers == null)
                {
                    answers = new ObservableCollection<Answer>();
                }
                return answers;
            }
            set { answers = value; }
        }

        public virtual void ReadXml(System.Xml.Linq.XElement element)
        {
            //CurrentID
            Question.CurrentID = element.ReadInt("QuCurrentID");
            Answer.CurrentID = element.ReadInt("AnCurrentID");
            //qustions
            XElement elem = element.Element("Questions");
            Questions.Clear();
            foreach (XElement e in elem.Elements())
            {
                Question qustion = new Question(true);

                qustion.ReadXml(e);


                Questions.Add(qustion);
            }
            //answers
            elem = element.Element("Answers");
            Answers.Clear();
            foreach (XElement e in elem.Elements())
            {
                Answer answer = new Answer(true);
                answer.ReadXml(e);
                Answers.Add(answer);

            }

        }
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            //questions
            writer.WriteStartElement("Call");
            writer.WriteAttributeInt("QuCurrentID", Question.CurrentID);
            writer.WriteAttributeInt("AnCurrentID",Answer.CurrentID);

            writer.WriteStartElement("Questions");
            foreach (Question question in Questions)
            {
                writer.WriteStartElement("Question");
                question.WriteXml(writer);
                writer.WriteEndElement();

            }
            writer.WriteEndElement();
            //answers
            writer.WriteStartElement("Answers");

            foreach (Answer answer in Answers)
            {
                writer.WriteStartElement("Answer");
                answer.WriteXml(writer);

                writer.WriteEndElement();

            }
            writer.WriteEndElement();
            writer.WriteEndElement();

        }
        public void SaveToXml(string FileName)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream,
                new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                })
                )
            {
                WriteXml(writer);

            }
            File.WriteAllText(FileName, Encoding.UTF8.GetString(stream.ToArray()));

        }
        public void LoadToXml(string FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                XElement elem = XElement.Load(FileName);
                ReadXml(elem);
            }


        }
    }
}
