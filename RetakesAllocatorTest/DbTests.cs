using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using RetakesAllocatorCore;
using RetakesAllocatorCore.Db;
using static RetakesAllocatorTest.TestConstants;

namespace RetakesAllocatorTest;

public class DbTests : BaseTestFixture
{
    [Test]
    public async Task TestGetUsersSettings()
    {
        var usersSettings = Queries.GetUsersSettings(new List<ulong>());
        Assert.That(usersSettings, Is.EqualTo(new Dictionary<ulong, UserSetting>()));

        await Queries.SetWeaponPreferenceForUserAsync(TestSteamId, CsTeam.Terrorist, WeaponAllocationType.HalfBuyPrimary,
            CsItem.Bizon);
        await Queries.SetWeaponPreferenceForUserAsync(TestSteamId, CsTeam.Terrorist, WeaponAllocationType.PistolRound, null);
        await Queries.SetWeaponPreferenceForUserAsync(TestSteamId, CsTeam.Terrorist, WeaponAllocationType.HalfBuyPrimary,
            CsItem.MP5);
        await Queries.SetWeaponPreferenceForUserAsync(TestSteamId, CsTeam.CounterTerrorist, WeaponAllocationType.FullBuyPrimary,
            CsItem.AK47);
        // Should set for both T and CT
        await Queries.SetPreferredWeaponPreferenceAsync(TestSteamId, CsItem.AWP);

        await Queries.SetWeaponPreferenceForUserAsync(2, CsTeam.Terrorist, WeaponAllocationType.FullBuyPrimary, CsItem.AK47);
        await Queries.SetWeaponPreferenceForUserAsync(2, CsTeam.Terrorist, WeaponAllocationType.Secondary, CsItem.Deagle);
        await Queries.SetWeaponPreferenceForUserAsync(2, CsTeam.CounterTerrorist, WeaponAllocationType.Secondary,
            CsItem.FiveSeven);
        // Will get different snipers for different teams
        await Queries.SetPreferredWeaponPreferenceAsync(2, CsItem.SCAR20);

        usersSettings = Queries.GetUsersSettings(new List<ulong> {TestSteamId});
        Assert.Multiple(() =>
        {
            Assert.That(usersSettings.Keys, Is.EquivalentTo(new List<ulong> {TestSteamId}));
            Assert.That(usersSettings.Values.Select(v => v.UserId), Is.EquivalentTo(new List<ulong> {TestSteamId}));
        });
        usersSettings = Queries.GetUsersSettings(new List<ulong> {2});
        Assert.Multiple(() =>
        {
            Assert.That(usersSettings.Keys, Is.EquivalentTo(new List<ulong> {2}));
            Assert.That(usersSettings.Values.Select(v => v.UserId), Is.EquivalentTo(new List<ulong> {2}));
        });
        usersSettings = Queries.GetUsersSettings(new List<ulong> {TestSteamId, 2});
        Assert.Multiple(() =>
        {
            Assert.That(usersSettings.Keys, Is.EquivalentTo(new List<ulong> {TestSteamId, 2}));
            Assert.That(usersSettings.Values.Select(v => v.UserId),
                Is.EquivalentTo(new List<ulong> {TestSteamId, 2}));

            Assert.That(
                usersSettings[TestSteamId].GetWeaponPreference(CsTeam.Terrorist, WeaponAllocationType.HalfBuyPrimary),
                Is.EqualTo(CsItem.MP5));
            Assert.That(
                usersSettings[TestSteamId].GetWeaponPreference(CsTeam.Terrorist, WeaponAllocationType.PistolRound),
                Is.EqualTo(null));
            Assert.That(
                usersSettings[TestSteamId]
                    .GetWeaponPreference(CsTeam.CounterTerrorist, WeaponAllocationType.FullBuyPrimary),
                Is.EqualTo(CsItem.AK47));
            Assert.That(
                usersSettings[TestSteamId]
                    .GetWeaponPreference(CsTeam.CounterTerrorist, WeaponAllocationType.HalfBuyPrimary),
                Is.EqualTo(null));
            Assert.That(
                usersSettings[TestSteamId]
                    .GetWeaponPreference(CsTeam.CounterTerrorist, WeaponAllocationType.Preferred),
                Is.EqualTo(CsItem.AWP));
            Assert.That(
                usersSettings[TestSteamId].GetWeaponPreference(CsTeam.Terrorist, WeaponAllocationType.Preferred),
                Is.EqualTo(CsItem.AWP));

            Assert.That(usersSettings[2].GetWeaponPreference(CsTeam.Terrorist, WeaponAllocationType.FullBuyPrimary),
                Is.EqualTo(CsItem.AK47));
            Assert.That(usersSettings[2].GetWeaponPreference(CsTeam.Terrorist, WeaponAllocationType.Secondary),
                Is.EqualTo(CsItem.Deagle));
            Assert.That(usersSettings[2].GetWeaponPreference(CsTeam.CounterTerrorist, WeaponAllocationType.Secondary),
                Is.EqualTo(CsItem.FiveSeven));
            Assert.That(usersSettings[2].GetWeaponPreference(CsTeam.Terrorist, WeaponAllocationType.Preferred),
                Is.EqualTo(CsItem.AutoSniperT));
            Assert.That(usersSettings[2].GetWeaponPreference(CsTeam.CounterTerrorist, WeaponAllocationType.Preferred),
                Is.EqualTo(CsItem.AutoSniperCT));
        });
    }
}
