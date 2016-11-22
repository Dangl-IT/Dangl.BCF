using System;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class ExternalBIMSnippetTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var Container = new BCFv21Container();
            Container.Topics.Add(CreateTopic());
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
                BimSnippet = new BimSnippet
                {
                    isExternal = true,
                    Reference = "http://bimfiles.example.com/JsonElement.json",
                    ReferenceSchema = "http://json-schema.org",
                    SnippetType = "JSON"
                },
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 06, 21, 12, 00, 00, DateTimeKind.Utc),
                Description = "This topic has an external BIM Snippet reference",
                Guid = BCFTestCaseData.ExternalBIMSnippet_TopicGuid,
                Index = 0,
                Title = "External BIM Snippet"
            };
            return Markup;
        }
    }
}