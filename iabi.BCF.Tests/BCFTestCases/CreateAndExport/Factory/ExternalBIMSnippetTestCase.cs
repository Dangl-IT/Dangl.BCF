﻿using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using System;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory
{
    public static class ExternalBIMSnippetTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var Container = new BCFv2Container();
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
                Index = "0",
                Title = "External BIM Snippet"
            };
            return Markup;
        }
    }
}