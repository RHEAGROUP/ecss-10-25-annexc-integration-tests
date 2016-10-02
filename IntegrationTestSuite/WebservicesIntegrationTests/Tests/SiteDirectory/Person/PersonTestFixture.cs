﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonTestFixture.cs" company="RHEA System">
//
//   Copyright 2016 RHEA System 
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WebservicesIntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The purpose of the <see cref="PersonTestFixture"/> is to execute integration tests using the GET and POST
    /// verbs on Person objects
    /// </summary>    
    public class PersonTestFixture: WebClientTestFixtureBase
    {
        /// <summary>
        /// Verification that the Person objects are returned from the data-source and that the 
        /// values of the person properties are equal to to expected value
        /// </summary>
        [Test]
        public void VerifyThatExpectedPersonIsReturnedFromWebApi()
        {
            // define the URI on which to perform a GET request
            var personsUri = new Uri(string.Format(UriFormat, this.Settings.Hostname, "/SiteDirectory/f13de6f8-b03a-46e7-a492-53b2f260f294/person"));

            // Get the response from the data-source as a JArray (JSON Array)
            var jArray = this.WebClient.GetDto(personsUri);
            
            // assert that the returned person count = 1
            Assert.AreEqual(1, jArray.Count);

            // get a specific person from the result by it's unique id
            var person = jArray.Single(x => (string)x["iid"] == "77791b12-4c2c-4499-93fa-869df3692d22");

            PersonTestFixture.VerifyProperties(person);
        }

        [Test]
        public void VerifyThatExpectedPersonWithContainerIsReturnedFromWebApi()
        {
            // define the URI on which to perform a GET request
            var personsUri = new Uri(string.Format(UriFormat, this.Settings.Hostname, "/SiteDirectory/f13de6f8-b03a-46e7-a492-53b2f260f294/person?includeAllContainers=true"));

            // Get the response from the data-source as a JArray (JSON Array)
            var jArray = this.WebClient.GetDto(personsUri);

            // assert that the returned person count = 2
            Assert.AreEqual(2, jArray.Count);

            var siteDirectory = jArray.Single(x => (string)x["iid"] == "f13de6f8-b03a-46e7-a492-53b2f260f294");
            SiteDirectoryTestFixture.VerifyProperties(siteDirectory);

            // get a specific person from the result by it's unique id
            var person = jArray.Single(x => (string)x["iid"] == "77791b12-4c2c-4499-93fa-869df3692d22");
            PersonTestFixture.VerifyProperties(person);
        }

        /// <summary>
        /// Verifies the properties of the Person <see cref="JToken"/>
        /// </summary>
        /// <param name="person">
        /// The <see cref="JToken"/> that contains the properties of
        /// the Person object
        /// </param>
        public static void VerifyProperties(JToken person)
        {
            // verify that the amount of returned properties 
            Assert.AreEqual(18, person.Children().Count());

            Assert.AreEqual("77791b12-4c2c-4499-93fa-869df3692d22", (string)person["iid"]);
            Assert.AreEqual(1, (int)person["revisionNumber"]);

            // assert that the properties are what is expected
            Assert.AreEqual("John", (string)person["givenName"]);
            Assert.AreEqual("Doe", (string)person["surname"]);
            Assert.AreEqual("", (string)person["organizationalUnit"]);
            Assert.AreEqual(null, (string)person["organization"]);
            Assert.AreEqual("0e92edde-fdff-41db-9b1d-f2e484f12535", (string)person["defaultDomain"]);
            Assert.IsTrue((bool)person["isActive"]);
            Assert.AreEqual("2428f4d9-f26d-4112-9d56-1c940748df69", (string)person["role"]);
            Assert.AreEqual(null, (string)person["defaultEmailAddress"]);
            Assert.AreEqual(null, (string)person["defaultTelephoneNumber"]);
            Assert.AreEqual("admin", (string)person["shortName"]);
            Assert.IsFalse((bool)person["isDeprecated"]);

            // verify that there is one email with the specified unique id
            var emails = (JArray)person["emailAddress"];
            IList<string> e = emails.Select(x => (string)x).ToList();
            Assert.IsEmpty(e);

            // verify that there are not telephoneNumbers for this person
            var telephoneNumbers = (JArray)person["telephoneNumber"];
            IList<string> t = telephoneNumbers.Select(x => (string)x).ToList();
            Assert.IsEmpty(t);

            // verify that there are no userPreference for this person
            var userPreferences = (JArray)person["userPreference"];
            IList<string> up = userPreferences.Select(x => (string)x).ToList();
            Assert.IsEmpty(up);
        }
    }
}
