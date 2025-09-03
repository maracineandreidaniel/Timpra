using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Timpra.BusinessLogic.Test.Utilities;

public partial class AutoDomainDataAttribute : AutoDataAttribute
{
    public AutoDomainDataAttribute()
        : base(() =>
            {
                var fixture = new Fixture()
                    .Customize(new CompositeCustomization(
                        new AutoNSubstituteCustomization { ConfigureMembers = true }
                    ));

                return fixture;
            }
        )
    {
    }
}
