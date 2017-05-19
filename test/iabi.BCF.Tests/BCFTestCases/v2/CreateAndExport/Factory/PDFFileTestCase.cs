using System;
using System.Collections.Generic;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory
{
    public static class PdfFileTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var container = new BCFv2Container();
            container.Topics.Add(CreateTopic());
            container.FileAttachments.Add("Requirements.pdf", TestCaseResourceFactory.GetFileAttachment(FileAttachments.RequirementsPdf));
            return container;
        }

        public static BCFTopic CreateTopic()
        {
            var returnTopic = new BCFTopic();
            returnTopic.Markup = CreateMarkup();
            return returnTopic;
        }

        private static Markup CreateMarkup()
        {
            var markup = new Markup();
            markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 11, 12, 13, 14, DateTimeKind.Utc),
                Description = "This topic has a document reference to an internal PDF file in the directory root.",
                Guid = BcFv2TestCaseData.PDF_FILE_TOPIC_GUID,
                Index = "0",
                Title = "PDF File",
                DocumentReferences = new List<TopicDocumentReferences>
                {
                    new TopicDocumentReferences
                    {
                        Description = "Project requirements (pdf)",
                        isExternal = false,
                        ReferencedDocument = "../Requirements.pdf"
                    }
                }
            };
            return markup;
        }
    }
}