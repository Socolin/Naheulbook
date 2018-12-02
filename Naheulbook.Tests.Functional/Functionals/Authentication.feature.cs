// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.0.0.0
//      SpecFlow Generator Version:3.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Naheulbook.Tests.Functional.Functionals
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Authentication")]
    public partial class AuthenticationFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Authentication.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Authentication", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Can create a user with a username and a password and validate his email")]
        public virtual void CanCreateAUserWithAUsernameAndAPasswordAndValidateHisEmail()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Can create a user with a username and a password and validate his email", null, ((string[])(null)));
#line 3
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line hidden
#line 4
    testRunner.When("performing a POST to the url \"/api/v2/users/\" with the following \"application/jso" +
                    "n\" content", "{\n  \"username\": \"test@naheulbook.fr\",\n  \"password\": \"iHE1vAqAKZtoPdWDXW9lgOkI+SWt" +
                    "GV/UB59fPU6Occ602wQs1xsOywVDPLy5z6DS\"\n}", ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 11
    testRunner.Then("the response status code is 201", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 12
    testRunner.And("a mail validation mail has been sent to \"test@naheulbook.fr\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 14
    testRunner.When("performing a POST to the url \"/api/v2/users/test@naheulbook.fr/validate\" with the" +
                    " following \"application/json\" content", "{\n  \"activationCode\": \"${ActivationCode}\"\n}", ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
    testRunner.Then("the response status code is 204", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A user can request a JWT")]
        public virtual void AUserCanRequestAJWT()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A user can request a JWT", null, ((string[])(null)));
#line 22
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 23
    testRunner.Given("a user identified by a password", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 25
    testRunner.When("performing a POST to the url \"/api/v2/users/${Username}/jwt\" with the following \"" +
                    "application/json\" content", "{\n  \"password\": \"${Password}\"\n}", ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
    testRunner.Then("the response status code is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 32
    testRunner.And("the response content contains a valid JWT", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion