using System;
using System.Collections.Generic;
using Dangl.BCF.BCFv21;
using Dangl.BCF.BCFv21.Schemas;

namespace Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class PdfFileTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var container = new BCFv21Container();
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
                Guid = BcFv21TestCaseData.PDF_FILE_TOPIC_GUID,
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
            return markup;
        }
    }
}