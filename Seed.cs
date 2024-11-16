using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seed
{
    public class SeedQuery
    {
        [JsonPropertyName("Index")]
        public int Index { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("API_ID")]
        public string API_ID { get; set; } = string.Empty;
        [JsonPropertyName("API_HASH")]
        public string API_HASH { get; set; } = string.Empty;
        [JsonPropertyName("Phone")]
        public string Phone { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        [JsonPropertyName("Active")]
        public bool Active { get; set; }
        [JsonPropertyName("DailyReward")]
        public bool DailyReward { get; set; }
        [JsonPropertyName("Seed")]
        public bool Seed { get; set; }
        [JsonPropertyName("Worm")]
        public bool Worm { get; set; }
        [JsonPropertyName("Egg")]
        public bool Egg { get; set; }
        [JsonPropertyName("Bird")]
        public bool Bird { get; set; }
        [JsonPropertyName("Boost")]
        public bool Boost { get; set; }
        [JsonPropertyName("Task")]
        public bool Task { get; set; }
        [JsonPropertyName("TaskSleep")]
        public int[]? TaskSleep { get; set; }
        [JsonPropertyName("DaySleep")]
        public int[]? DaySleep { get; set; }
        [JsonPropertyName("NightSleep")]
        public int[]? NightSleep { get; set; }
    }

    public class SeedProfile2Response
    {
        [JsonPropertyName("data")]
        public SeedProfile2Data? Data { get; set; }
    }

    public class SeedProfile2Data
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("tg_id")]
        public long TGId { get; set; }
        [JsonPropertyName("upgrades")]
        public List<SeedProfile2DataUpgrades>? Upgrades { get; set; }
        [JsonPropertyName("last_claim")]
        public DateTime LastClaim { get; set; }
        [JsonPropertyName("give_first_egg")]
        public bool GiveFirstEgg { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("bonus_claimed")]
        public bool BonusClaimed { get; set; }
    }

    public class SeedProfile2DataUpgrades
    {
        [JsonPropertyName("upgrade_type")]
        public string UpgradeType { get; set; } = string.Empty;
    }

    public class SeedBalanceResponse
    {
        [JsonPropertyName("data")]
        public long Data { get; set; }
    }

    public class SeedWormsResponse
    {
        [JsonPropertyName("data")]
        public SeedWormsData? Data { get; set; }
    }

    public class SeedWormsData
    {
        [JsonPropertyName("type")]
        public string @type { get; set; } = string.Empty;
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("ended_at")]
        public DateTime EndedAt { get; set; }
        [JsonPropertyName("next_worm")]
        public DateTime NextWorm { get; set; }
        [JsonPropertyName("reward")]
        public long Reward { get; set; }
        [JsonPropertyName("is_caught")]
        public bool IsCaught { get; set; }
    }

    public class SeedStreakRewardRequest
    {
        [JsonPropertyName("streak_reward_ids")]
        public List<string>? StreakRewardIds { get; set; }
    }

    public class SeedStreakRewardResponse
    {
        [JsonPropertyName("data")]
        public List<SeedStreakRewardData>? Data { get; set; }
    }

    public class SeedStreakRewardData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("no")]
        public int No { get; set; }
    }

    public class SeedTasksResponse
    {
        [JsonPropertyName("data")]
        public List<SeedTasksData>? Data { get; set; }
    }

    public class SeedTasksData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("reward_amount")]
        public long RewardAmount { get; set; }
        [JsonPropertyName("task_user")]
        public SeedTasksDataTaskUser? TaskUser { get; set; }
    }

    public class SeedTasksDataTaskUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }

    public class SeedGuildResponse
    {
        [JsonPropertyName("data")]
        public SeedGuildData? Data { get; set; }
    }

    public class SeedGuildData
    {
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; } = string.Empty;
        [JsonPropertyName("member_id")]
        public string MemberId { get; set; } = string.Empty;
        [JsonPropertyName("hunted")]
        public long Hunted { get; set; }
        [JsonPropertyName("is_master")]
        public bool IsMaster { get; set; }
        [JsonPropertyName("member_rank")]
        public int MemberRank { get; set; }
    }

    public class SeedGuildRequest
    {
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; } = string.Empty;
    }

    public class ProxyType
    {
        [JsonPropertyName("Index")]
        public int Index { get; set; }
        [JsonPropertyName("Proxy")]
        public string Proxy { get; set; } = string.Empty;
    }

    public class Httpbin
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;
    }
}