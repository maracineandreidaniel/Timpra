using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Entities;
using NSubstitute;

namespace Timpra.API.Test.Utilities;

public partial class AutoDomainDataAttribute : AutoDataAttribute
{
    public AutoDomainDataAttribute()
        : base(
            () => new Fixture()
                    .Customize(new CompositeCustomization(
                        new AutoNSubstituteCustomization { ConfigureMembers = true }
                    ))
        )
    {
    }
}
