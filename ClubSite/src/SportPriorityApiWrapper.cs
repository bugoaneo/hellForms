using NPoco;
using SportPriority.ClubSite.Api.Client;
using SportPriority.ClubSite.Api.DataContracts.Wg;
using System.Runtime.Caching;
using Umbraco.Cms.Core.Models.PublishedContent;
using static ClubSite.Configuration;

namespace ClubSite
{
    public class SportPriorityApiWrapper
    {
        protected static ApiClient GetApiClient()
        {
            //TODO: вроде, здесь надо выбирать клиента из пула клиентов.
            return new ApiClient(Configuration.GetApiBaseUrl());
        }


        public static IEnumerable<CoachInfo> GetCoaches(string clubId, string coachFilterStr)
        {            

            var coachesCacheKey = "coaches-" + clubId;

            var cache = MemoryCache.Default;

            IEnumerable<CoachInfo>? coachesCached = cache[coachesCacheKey] as IEnumerable<CoachInfo>;

            if (coachesCached == null)
            {

                coachesCached = Task.Run(() =>
                {
                    return GetApiClient().GetCoachesByClub(new GetByClub { Id = new Guid(clubId) });
                })
                .Result.Result;

                cache.Add(coachesCacheKey, coachesCached, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(10)});

            }
            if (coachFilterStr.Length > 0)
            {
                try
                {
                    var regex = new System.Text.RegularExpressions.Regex(coachFilterStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    var coachesFiltered = coachesCached.Where(x => !string.IsNullOrEmpty(x.Group) && regex.IsMatch(x.Group));
                    return coachesFiltered;
                }
                catch { }
            }
            return coachesCached;
        }
        
        public static IEnumerable<CourseInfo> GetCourses(string clubId)
        {            

            var coursesCacheKey = "courses-" + clubId;

            var cache = MemoryCache.Default;

            IEnumerable<CourseInfo>? coursesCached = cache[coursesCacheKey] as IEnumerable<CourseInfo>;

            if (coursesCached == null)
            {

                coursesCached = Task.Run(() =>
                {
                    return GetApiClient().GetCoursesByClub(new GetByClub { Id = new Guid(clubId) });
                })
                .Result.Result;

                cache.Add(coursesCacheKey, coursesCached, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(10)});

            }
            return coursesCached;
        }

        public static IEnumerable<SubscriptionPlanInfo> GetPlans(string clubId)
        {

            var plansCacheKey = "plans-" + clubId;

            var cache = MemoryCache.Default;

            IEnumerable<SubscriptionPlanInfo>? plansCached = cache[plansCacheKey] as IEnumerable<SubscriptionPlanInfo>;

            if (plansCached == null)
            {

                plansCached = Task.Run(() =>
                {
                    return GetApiClient().GetSubscriptionsByClub(new GetByClub { Id = new Guid(clubId) });
                })
                .Result.Result;

                cache.Add(plansCacheKey, plansCached, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(10) });

            }
            return plansCached;
        }
    }
}
