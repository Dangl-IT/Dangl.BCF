using System;
using System.Collections.Generic;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class PDFFileTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var Container = new BCFv21Container();
            Container.Topics.Add(CreateTopic());
            Container.FileAttachments.Add("Requirements.pdf", TestCaseResourceFactory.GetFileAttachment(FileAttachments.RequirementsPdf));
            return Container;
        }

        public static BCFTopic CreateTopic()
        {
            var ReturnTopic = new BCFTopic();
            ReturnTopic.Markup = CreateMarkup();
            return ReturnTopic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();
            Markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 11, 12, 13, 14, DateTimeKind.Utc),
                Description = "This topic has a document reference to an internal PDF file in the directory root.",
                Guid = BCFv21TestCaseData.PDFFile_TopicGuid,
                Index = 0,
                Title = "PDF File",
                DocumentReference = new List<TopicDocumentReference>
                {
                    new TopicDocumentReference
                    {
                        Description = "Project requirements (pdf)",
                        isExternal = false,
                        ReferencedDocument = "../Requirements.pdf"
                    }
                }
            };
            return Markup;
        }
    }
}