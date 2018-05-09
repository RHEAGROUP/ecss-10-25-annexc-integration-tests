﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetTestFixture.cs" company="RHEA System">
//   Copyright 2016 RHEA System 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebservicesIntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class ParameterValueSetTestFixture : WebClientTestFixtureBase
    {
        public override void SetUp()
        {
            base.SetUp();

            this.WebClient.Restore(this.Settings.Hostname);
        }

        public override void TearDown()
        {
            this.WebClient.Restore(this.Settings.Hostname);

            base.TearDown();
        }

        /// <summary>
        /// Verification that the ParameterValueSet objects are returned from the data-source and that the 
        /// values of the ParameterValueSet properties are equal to the expected value
        /// </summary>
        [Test]
        public void VerifyThatExpectedParameterValueSetIsReturnedFromWebApi()
        {
            // define the URI on which to perform a GET request 
            var parameterValueSetUri =
                new Uri(
                    string.Format(
                        UriFormat,
                        this.Settings.Hostname,
                        "/EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c/element/f73860b2-12f0-43e4-b8b2-c81862c0a159/parameter/6c5aff74-f983-4aa8-a9d6-293b3429307c/valueSet"));

            // get a response from the data-source as a JArray (JSON Array)
            var jArray = this.WebClient.GetDto(parameterValueSetUri);

            // check if there is the only one ParameterValueSet object 
            Assert.AreEqual(1, jArray.Count);

            // get a specific ParameterValueSet from the result by it's unique id
            var parameterValueSet =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "af5c88c6-301f-497b-81f7-53748c3900ed");

            ParameterValueSetTestFixture.VerifyProperties(parameterValueSet);
        }

        [Test]
        public void VerifyThatExpectedParameterValueSetWithContainerIsReturnedFromWebApi()
        {
            // define the URI on which to perform a GET request
            var parameterValueSetUri =
                new Uri(
                    string.Format(
                        UriFormat,
                        this.Settings.Hostname,
                        "/EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c/element/f73860b2-12f0-43e4-b8b2-c81862c0a159/parameter/6C5AFF74-F983-4AA8-A9D6-293B3429307C/valueSet?includeAllContainers=true"));

            // get a response from the data-source as a JArray (JSON Array)
            var jArray = this.WebClient.GetDto(parameterValueSetUri);

            // check if there are 5 objects
            Assert.AreEqual(5, jArray.Count);

            // get a specific Iteration from the result by it's unique id
            var iteration = jArray.Single(x => (string)x[PropertyNames.Iid] == "e163c5ad-f32b-4387-b805-f4b34600bc2c");
            IterationTestFixture.VerifyProperties(iteration);

            // get a specific ElementDefinition from the result by it's unique id
            ElementDefinitionTestFixture.VerifyProperties(jArray);

            // get a specific Parameter from the result by it's unique id
            var parameter = jArray.Single(x => (string)x[PropertyNames.Iid] == "6c5aff74-f983-4aa8-a9d6-293b3429307c");
            ParameterTestFixture.VerifyProperties(parameter);

            // get a specific ParameterValueSet from the result by it's unique id
            var parameterValueSet =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "af5c88c6-301f-497b-81f7-53748c3900ed");
            ParameterValueSetTestFixture.VerifyProperties(parameterValueSet);
        }

        [Test]
        public void VerifyThatAParameteValueSetCanBeDeletedAndCreatedtWithWebApi()
        {
            var iterationUri =
                new Uri(
                    string.Format(
                        UriFormat,
                        this.Settings.Hostname,
                        "/EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c"));
            var postBodyPath =
                this.GetPath("Tests/EngineeringModel/ParameterValueSet/PostNewParameterValueSetAndDeleteOne.json");

            var postBody = this.GetJsonFromFile(postBodyPath);
            var jArray = this.WebClient.PostDto(iterationUri, postBody);

            var engineeeringModel =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "9ec982e4-ef72-4953-aa85-b158a95d8d56");
            Assert.AreEqual(2, (int)engineeeringModel[PropertyNames.RevisionNumber]);

            var parameterSubscription =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "f1f076c4-5307-42b8-a171-3263a9e7bb21");
            Assert.AreEqual(2, (int)parameterSubscription[PropertyNames.RevisionNumber]);

            var parameterOverride =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "93f767ed-4d22-45f6-ae97-d1dab0d36e1c");
            Assert.AreEqual(2, (int)parameterOverride[PropertyNames.RevisionNumber]);

            var parameter = jArray.Single(x => (string)x[PropertyNames.Iid] == "6c5aff74-f983-4aa8-a9d6-293b3429307c");
            Assert.AreEqual(2, (int)parameter[PropertyNames.RevisionNumber]);

            var expectedValueSets = new[] { "d2936657-95b3-4b27-bf98-a19752dc2c7f" };
            var valueSetsArray = (JArray)parameter[PropertyNames.ValueSet];
            IList<string> valueSets = valueSetsArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedValueSets, valueSets);

            var parameterValueSet =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "d2936657-95b3-4b27-bf98-a19752dc2c7f");
            Assert.AreEqual(2, (int)parameter[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ParameterValueSet", (string)parameterValueSet[PropertyNames.ClassKind]);

            Assert.AreEqual("MANUAL", (string)parameterValueSet[PropertyNames.ValueSwitch]);

            const string emptyProperty = "[\"-\"]";
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Published]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Formula]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Computed]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Manual]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Reference]);

            Assert.IsNull((string)parameterValueSet[PropertyNames.ActualState]);
            Assert.IsNull((string)parameterValueSet[PropertyNames.ActualOption]);
        }

        [Test]
        public void VerifyThatAParameteValueSetIsCreatedWhenPossibleStateIsAddedWithWebApi()
        {
            // POST state dependent Parameter and check what is returned
            var iterationUri =
                new Uri(
                    string.Format(
                        UriFormat,
                        this.Settings.Hostname,
                        "/EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c"));
            var postBodyPath =
                this.GetPath("Tests/EngineeringModel/ParameterValueSet/PostNewStateDependentParameter.json");

            var postBody = this.GetJsonFromFile(postBodyPath);
            var jArray = this.WebClient.PostDto(iterationUri, postBody);

            var engineeeringModel =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "9ec982e4-ef72-4953-aa85-b158a95d8d56");
            Assert.AreEqual(2, (int)engineeeringModel[PropertyNames.RevisionNumber]);

            // get a specific ElementDefinition from the result by it's unique id
            var elementDefinition =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "f73860b2-12f0-43e4-b8b2-c81862c0a159");
            Assert.AreEqual(2, (int)elementDefinition[PropertyNames.RevisionNumber]);

            var expectedParameters = new[]
                                         {
                                             "6c5aff74-f983-4aa8-a9d6-293b3429307c",
                                             "3f05483f-66ff-4f21-bc76-45956779f66e",
                                             "2cd4eb9c-e92c-41b2-968c-f03ff7010bad"
                                         };
            var parametersArray = (JArray)elementDefinition[PropertyNames.Parameter];
            IList<string> parameters = parametersArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedParameters, parameters);

            // get a specific Parameter from the result by it's unique id
            var parameter = jArray.Single(x => (string)x[PropertyNames.Iid] == "2cd4eb9c-e92c-41b2-968c-f03ff7010bad");
            Assert.AreEqual(2, (int)parameter[PropertyNames.RevisionNumber]);

            var valueSetsArray = (JArray)parameter[PropertyNames.ValueSet];
            IList<string> valueSets = valueSetsArray.Select(x => (string)x).ToList();
            Assert.AreEqual(1, valueSets.Count);

            var parameterValueSet = jArray.Single(x => (string)x[PropertyNames.Iid] == valueSets[0]);
            Assert.AreEqual(2, (int)parameter[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ParameterValueSet", (string)parameterValueSet[PropertyNames.ClassKind]);

            Assert.AreEqual("MANUAL", (string)parameterValueSet[PropertyNames.ValueSwitch]);

            const string EmptyProperty = "[\"-\"]";
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet[PropertyNames.Published]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet[PropertyNames.Formula]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet[PropertyNames.Computed]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet[PropertyNames.Manual]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet[PropertyNames.Reference]);

            Assert.AreEqual(
                "b91bfdbb-4277-4a03-b519-e4db839ef5d4",
                (string)parameterValueSet[PropertyNames.ActualState]);
            Assert.IsNull((string)parameterValueSet[PropertyNames.ActualOption]);

            // POST PossibleFiniteState and check what is returned
            postBodyPath = this.GetPath("Tests/EngineeringModel/ParameterValueSet/PostNewPossibleFiniteState.json");

            postBody = this.GetJsonFromFile(postBodyPath);
            jArray = this.WebClient.PostDto(iterationUri, postBody);

            engineeeringModel =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "9ec982e4-ef72-4953-aa85-b158a95d8d56");
            Assert.AreEqual(3, (int)engineeeringModel[PropertyNames.RevisionNumber]);

            // get a specific ActualFiniteStateList from the result by it's unique id
            var actualFiniteStateList =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "db690d7d-761c-47fd-96d3-840d698a89dc");
            Assert.AreEqual(3, (int)actualFiniteStateList[PropertyNames.RevisionNumber]);

            var actualStatesArray = (JArray)actualFiniteStateList[PropertyNames.ActualState];
            IList<string> actualStates = actualStatesArray.Select(x => (string)x).ToList();
            Assert.AreEqual(2, actualStates.Count);

            // get an ActualFiniteState from the result by it's unique id for existed PossibleFiniteState
            var actualFiniteState0 = jArray.Single(x => (string)x[PropertyNames.Iid] == actualStates[0]);
            Assert.AreEqual(3, (int)actualFiniteState0[PropertyNames.RevisionNumber]);

            Assert.AreEqual(3, (int)actualFiniteState0[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ActualFiniteState", (string)actualFiniteState0[PropertyNames.ClassKind]);
            Assert.AreEqual("MANDATORY", (string)actualFiniteState0[PropertyNames.Kind]);

            var expectedPossibleStates = new[] { "b8fdfac4-1c40-475a-ac6c-968654b689b6" };
            var possibleStateArray = (JArray)actualFiniteState0[PropertyNames.PossibleState];
            IList<string> possibleStates = possibleStateArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedPossibleStates, possibleStates);

            // get a specific ActualFiniteState from the result by it's unique id
            var actualFiniteState1 = jArray.Single(x => (string)x[PropertyNames.Iid] == actualStates[1]);

            Assert.AreEqual(3, (int)actualFiniteState1[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ActualFiniteState", (string)actualFiniteState1[PropertyNames.ClassKind]);
            Assert.AreEqual("MANDATORY", (string)actualFiniteState1[PropertyNames.Kind]);

            expectedPossibleStates = new[] { "b8fdfac4-1c40-475a-ac6c-968654b689b7" };
            possibleStateArray = (JArray)actualFiniteState1[PropertyNames.PossibleState];
            possibleStates = possibleStateArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedPossibleStates, possibleStates);

            // get a specific PossibleFiniteStateList from the result by it's unique id
            var possibleFiniteStateList =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "449a5bca-34fd-454a-93f8-a56ac8383fee");
            Assert.AreEqual(3, (int)possibleFiniteStateList[PropertyNames.RevisionNumber]);
            var expectedPossibleFiniteStates = new List<OrderedItem>
                                                   {
                                                       new OrderedItem(73203278, "b8fdfac4-1c40-475a-ac6c-968654b689b6"),
                                                       new OrderedItem(73203279, "b8fdfac4-1c40-475a-ac6c-968654b689b7")
                                                   };
            var possibleFiniteStates =
                JsonConvert.DeserializeObject<List<OrderedItem>>(
                    possibleFiniteStateList[PropertyNames.PossibleState].ToString());
            CollectionAssert.AreEquivalent(expectedPossibleFiniteStates, possibleFiniteStates);

            // get a specific PossibleFiniteState from the result by it's unique id
            var possibleFiniteState =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "b8fdfac4-1c40-475a-ac6c-968654b689b7");
            Assert.AreEqual(3, (int)possibleFiniteState[PropertyNames.RevisionNumber]);
            Assert.AreEqual("PossibleFiniteState", (string)possibleFiniteState[PropertyNames.ClassKind]);
            Assert.AreEqual("Test PossibleFiniteState", (string)possibleFiniteState[PropertyNames.Name]);
            Assert.AreEqual("TestPossibleFiniteState", (string)possibleFiniteState[PropertyNames.ShortName]);

            var expectedAliases = new string[] { };
            var aliasesArray = (JArray)possibleFiniteState[PropertyNames.Alias];
            IList<string> aliases = aliasesArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedAliases, aliases);

            var expectedDefinitions = new string[] { };
            var definitionsArray = (JArray)possibleFiniteState[PropertyNames.Definition];
            IList<string> definitions = definitionsArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedDefinitions, definitions);

            var expectedHyperlinks = new string[] { };
            var hyperlinksArray = (JArray)possibleFiniteState[PropertyNames.HyperLink];
            IList<string> h = hyperlinksArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedHyperlinks, h);

            // get a specific Parameter from the result by it's unique id
            parameter = jArray.Single(x => (string)x[PropertyNames.Iid] == "2cd4eb9c-e92c-41b2-968c-f03ff7010bad");
            Assert.AreEqual(3, (int)parameter[PropertyNames.RevisionNumber]);

            valueSetsArray = (JArray)parameter[PropertyNames.ValueSet];
            valueSets = valueSetsArray.Select(x => (string)x).ToList();
            Assert.AreEqual(2, valueSets.Count);

            var parameterValueSet0 = jArray.Single(x => (string)x[PropertyNames.Iid] == valueSets[0]);
            Assert.AreEqual(3, (int)parameterValueSet0[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ParameterValueSet", (string)parameterValueSet0[PropertyNames.ClassKind]);

            Assert.AreEqual("MANUAL", (string)parameterValueSet0[PropertyNames.ValueSwitch]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet0[PropertyNames.Published]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet0[PropertyNames.Formula]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet0[PropertyNames.Computed]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet0[PropertyNames.Manual]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet0[PropertyNames.Reference]);

            Assert.AreEqual(actualStates[0], (string)parameterValueSet0[PropertyNames.ActualState]);
            Assert.IsNull((string)parameterValueSet0[PropertyNames.ActualOption]);

            var parameterValueSet1 = jArray.Single(x => (string)x[PropertyNames.Iid] == valueSets[1]);
            Assert.AreEqual(3, (int)parameterValueSet1[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ParameterValueSet", (string)parameterValueSet1[PropertyNames.ClassKind]);

            Assert.AreEqual("MANUAL", (string)parameterValueSet1[PropertyNames.ValueSwitch]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet1[PropertyNames.Published]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet1[PropertyNames.Formula]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet1[PropertyNames.Computed]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet1[PropertyNames.Manual]);
            Assert.AreEqual(EmptyProperty, (string)parameterValueSet1[PropertyNames.Reference]);

            Assert.AreEqual(actualStates[1], (string)parameterValueSet1[PropertyNames.ActualState]);
            Assert.IsNull((string)parameterValueSet1[PropertyNames.ActualOption]);

            // POST PossibleFiniteStateList and check what is returned
            postBodyPath = this.GetPath("Tests/EngineeringModel/ParameterValueSet/PostNewPossibleFiniteStateList.json");

            postBody = this.GetJsonFromFile(postBodyPath);
            jArray = this.WebClient.PostDto(iterationUri, postBody);

            engineeeringModel =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "9ec982e4-ef72-4953-aa85-b158a95d8d56");
            Assert.AreEqual(4, (int)engineeeringModel[PropertyNames.RevisionNumber]);

            // get a specific Iteration from the result by it's unique id
            var iteration = jArray.Single(x => (string)x[PropertyNames.Iid] == "e163c5ad-f32b-4387-b805-f4b34600bc2c");
            Assert.AreEqual(4, (int)iteration[PropertyNames.RevisionNumber]);

            var expectedPossibleFiniteStateLists = new[]
                                                       {
                                                           "449a5bca-34fd-454a-93f8-a56ac8383fee",
                                                           "dc3e3763-b8ed-4159-acee-d6a0f4de3dba"
                                                       };
            var possibleFiniteStateListsArray = (JArray)iteration[PropertyNames.PossibleFiniteStateList];
            IList<string> possibleFiniteStateLists = possibleFiniteStateListsArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedPossibleFiniteStateLists, possibleFiniteStateLists);

            // get a specific PossibleFiniteStateList from the result by it's unique id
            possibleFiniteStateList =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "dc3e3763-b8ed-4159-acee-d6a0f4de3dba");

            Assert.AreEqual(4, (int)possibleFiniteStateList[PropertyNames.RevisionNumber]);
            Assert.AreEqual("PossibleFiniteStateList", (string)possibleFiniteStateList[PropertyNames.ClassKind]);

            Assert.AreEqual("PossibleFiniteStateList Test", (string)possibleFiniteStateList[PropertyNames.Name]);
            Assert.AreEqual("PossibleFiniteStateListTest", (string)possibleFiniteStateList[PropertyNames.ShortName]);

            Assert.AreEqual(
                "0e92edde-fdff-41db-9b1d-f2e484f12535",
                (string)possibleFiniteStateList[PropertyNames.Owner]);

            expectedPossibleFiniteStates = new List<OrderedItem>
                                               {
                                                   new OrderedItem(-2577538, "d9ed3b43-ba45-45d5-ba1b-196703998a01")
                                               };
            possibleFiniteStates =
                JsonConvert.DeserializeObject<List<OrderedItem>>(
                    possibleFiniteStateList[PropertyNames.PossibleState].ToString());
            CollectionAssert.AreEquivalent(expectedPossibleFiniteStates, possibleFiniteStates);

            Assert.IsNull((string)possibleFiniteStateList[PropertyNames.DefaultState]);

            var expectedCategories = new string[] { };
            var categoriesArray = (JArray)possibleFiniteStateList[PropertyNames.Category];
            IList<string> categories = categoriesArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedCategories, categories);

            expectedAliases = new string[] { };
            aliasesArray = (JArray)possibleFiniteStateList[PropertyNames.Alias];
            aliases = aliasesArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedAliases, aliases);

            expectedDefinitions = new string[] { };
            definitionsArray = (JArray)possibleFiniteStateList[PropertyNames.Definition];
            definitions = definitionsArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedDefinitions, definitions);

            expectedHyperlinks = new string[] { };
            hyperlinksArray = (JArray)possibleFiniteStateList[PropertyNames.HyperLink];
            h = hyperlinksArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedHyperlinks, h);

            // get a specific PossibleFiniteState from the result by it's unique id
            possibleFiniteState =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "d9ed3b43-ba45-45d5-ba1b-196703998a01");
            Assert.AreEqual(4, (int)possibleFiniteState[PropertyNames.RevisionNumber]);
            Assert.AreEqual("PossibleFiniteState", (string)possibleFiniteState[PropertyNames.ClassKind]);
            Assert.AreEqual("PossibleFiniteState Test", (string)possibleFiniteState[PropertyNames.Name]);
            Assert.AreEqual("PossibleFiniteStateTest", (string)possibleFiniteState[PropertyNames.ShortName]);

            expectedAliases = new string[] { };
            aliasesArray = (JArray)possibleFiniteState[PropertyNames.Alias];
            aliases = aliasesArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedAliases, aliases);

            expectedDefinitions = new string[] { };
            definitionsArray = (JArray)possibleFiniteState[PropertyNames.Definition];
            definitions = definitionsArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedDefinitions, definitions);

            expectedHyperlinks = new string[] { };
            hyperlinksArray = (JArray)possibleFiniteState[PropertyNames.HyperLink];
            h = hyperlinksArray.Select(x => (string)x).ToList();
            CollectionAssert.AreEquivalent(expectedHyperlinks, h);

            // get a specific ActualFiniteStateList from the result by it's unique id
            actualFiniteStateList =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "db690d7d-761c-47fd-96d3-840d698a89dc");
            Assert.AreEqual(4, (int)actualFiniteStateList[PropertyNames.RevisionNumber]);
            actualStatesArray = (JArray)actualFiniteStateList[PropertyNames.ActualState];
            actualStates = actualStatesArray.Select(x => (string)x).ToList();
            Assert.AreEqual(2, actualStates.Count);
            var expectedPossibleFiniteStateListsInActualFiniteStateList = new List<OrderedItem>
                                                                              {
                                                                                  new OrderedItem(
                                                                                      4598335,
                                                                                      "449a5bca-34fd-454a-93f8-a56ac8383fee"),
                                                                                  new OrderedItem(
                                                                                      4598336,
                                                                                      "dc3e3763-b8ed-4159-acee-d6a0f4de3dba")
                                                                              };
            var possibleFiniteStateListsInActualFiniteStateList =
                JsonConvert.DeserializeObject<List<OrderedItem>>(
                    actualFiniteStateList[PropertyNames.PossibleFiniteStateList].ToString());
            CollectionAssert.AreEquivalent(
                expectedPossibleFiniteStateListsInActualFiniteStateList,
                possibleFiniteStateListsInActualFiniteStateList);

            // get a specific ActualFiniteState from the result by it's unique id
            actualFiniteState0 = jArray.Single(x => (string)x[PropertyNames.Iid] == actualStates[0]);
            Assert.AreEqual(4, (int)actualFiniteState0[PropertyNames.RevisionNumber]);

            actualFiniteState1 = jArray.Single(x => (string)x[PropertyNames.Iid] == actualStates[1]);
            Assert.AreEqual(4, (int)actualFiniteState1[PropertyNames.RevisionNumber]);

            parameterValueSet = jArray.Single(x => (string)x[PropertyNames.ClassKind] == "ParameterValueSet" && (string)x[PropertyNames.ActualState] == actualStates[0]);
            Assert.AreEqual(4, (int)parameterValueSet[PropertyNames.RevisionNumber]);

            parameterValueSet = jArray.Single(x => (string)x[PropertyNames.ClassKind] == "ParameterValueSet" && (string)x[PropertyNames.ActualState] == actualStates[1]);
            Assert.AreEqual(4, (int)parameterValueSet[PropertyNames.RevisionNumber]);

            // get a specific Parameter from the result by it's unique id
            parameter = jArray.Single(x => (string)x[PropertyNames.Iid] == "2cd4eb9c-e92c-41b2-968c-f03ff7010bad");
            Assert.AreEqual(4, (int)parameter[PropertyNames.RevisionNumber]);
        }

        /// <summary>
        /// Verifies all properties of the ParameterValueSet <see cref="JToken"/>
        /// </summary>
        /// <param name="parameterValueSet">
        /// The <see cref="JToken"/> that contains the properties of
        /// the ParameterValueSet object
        /// </param>
        public static void VerifyProperties(JToken parameterValueSet)
        {
            // verify the amount of returned properties 
            Assert.AreEqual(11, parameterValueSet.Children().Count());

            // assert that the properties are what is expected
            Assert.AreEqual("af5c88c6-301f-497b-81f7-53748c3900ed", (string)parameterValueSet[PropertyNames.Iid]);
            Assert.AreEqual(1, (int)parameterValueSet[PropertyNames.RevisionNumber]);
            Assert.AreEqual("ParameterValueSet", (string)parameterValueSet[PropertyNames.ClassKind]);

            Assert.AreEqual("MANUAL", (string)parameterValueSet[PropertyNames.ValueSwitch]);

            const string emptyProperty = "[\"-\"]";
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Published]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Formula]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Computed]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Manual]);
            Assert.AreEqual(emptyProperty, (string)parameterValueSet[PropertyNames.Reference]);

            Assert.IsNull((string)parameterValueSet[PropertyNames.ActualState]);
            Assert.IsNull((string)parameterValueSet[PropertyNames.ActualOption]);
        }

        [Test]
        public void Verify_that_the_computed_and_formula_property_of_a_ParameterValueSet_can_updated()
        {
            // POST state dependent Parameter and check what is returned
            var iterationUri =
                new Uri(
                    string.Format(
                        UriFormat,
                        this.Settings.Hostname,
                        "/EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c"));
            var postBodyPath =
                this.GetPath("Tests/EngineeringModel/ParameterValueSet/PostUpdateComputedValueOfParameterValueSet.json");

            var postBody = this.GetJsonFromFile(postBodyPath);
            
        
            var jArray = this.WebClient.PostDto(iterationUri, postBody);

            var engineeeringModel =
                jArray.Single(x => (string)x[PropertyNames.Iid] == "9ec982e4-ef72-4953-aa85-b158a95d8d56");
            Assert.AreEqual(2, (int)engineeeringModel[PropertyNames.RevisionNumber]);

            var parameterValueSet = jArray.Single(x => (string)x[PropertyNames.Iid] == "72ec3701-bcb5-4bf6-bd78-30fd1b65e3be");

            Assert.AreEqual("72ec3701-bcb5-4bf6-bd78-30fd1b65e3be", (string)parameterValueSet[PropertyNames.Iid]);
        }
    }
}