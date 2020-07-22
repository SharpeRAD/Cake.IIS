#region Using Statements
using Microsoft.Web.Administration;

using Xunit;
using Shouldly;
#endregion



namespace Cake.IIS.Tests
{
    public class RewriteRuleTests
    {
        [Fact(Skip = "https://forums.iis.net/t/1200296.aspx")]
        public void Should_Create_Delete_RewriteRule()
        {
            // Arrange
            const string ruleName = "force https";
            var settings = CakeHelper.GetRewriteRuleSettings(ruleName);

            //Don't exists
            CakeHelper.ExistsRewriteRule(ruleName).ShouldBeFalse();

            //Try to delete
            CakeHelper.DeleteRewriteRule(ruleName).ShouldBeFalse();

            // Create
            CakeHelper.CreateRewriteRule(settings);


            //Exists
            CakeHelper.ExistsRewriteRule(ruleName).ShouldBeTrue();

            //Delete
            CakeHelper.DeleteRewriteRule(ruleName).ShouldBeTrue();
        }
    }
}