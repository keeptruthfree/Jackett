using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Jackett.Common.Models;
using Jackett.Common.Models.IndexerConfig.Bespoke;
using Jackett.Common.Services.Interfaces;
using Jackett.Common.Utils;
using Jackett.Common.Utils.Clients;
using Newtonsoft.Json.Linq;
using NLog;

namespace Jackett.Common.Indexers
{
    internal class EliteTracker : BaseWebIndexer
    {
        private string LoginUrl => SiteLink + "takelogin.php";
        private string BrowseUrl => SiteLink + "browse.php";
        private bool TorrentHTTPSMode => configData.TorrentHTTPSMode.Value;
        private string ReplaceMulti => configData.ReplaceMulti.Value;
        private new ConfigurationDataEliteTracker configData

        {
            get => (ConfigurationDataEliteTracker)base.configData;
            set => base.configData = value;
        }

        public EliteTracker(IIndexerConfigurationService configService, WebClient webClient, Logger logger, IProtectionService protectionService)
            : base(name: "Elite-Tracker",
                description: "French Torrent Tracker",
                link: "https://elite-tracker.net/",
                caps: new TorznabCapabilities(),
                configService: configService,
                logger: logger,
                p: protectionService,
                client: webClient,
                configData: new ConfigurationDataEliteTracker()
                )
        {
            Encoding = Encoding.UTF8;
            Language = "fr-fr";
            Type = "private";

            // Clean capabilities
            TorznabCaps.Categories.Clear();

            AddCategoryMapping(27, TorznabCatType.TVAnime, "Animation/Animes");
            AddCategoryMapping(63, TorznabCatType.TVAnime, "Animes DVD");
            AddCategoryMapping(56, TorznabCatType.TVAnime, "Animes HD");
            AddCategoryMapping(59, TorznabCatType.TVAnime, "Animes Serie");
            AddCategoryMapping(89, TorznabCatType.TVAnime, "Animes HDLight");
            AddCategoryMapping(87, TorznabCatType.TVAnime, "Animes Pack");
            AddCategoryMapping(88, TorznabCatType.TVAnime, "Animes SD");
            AddCategoryMapping(90, TorznabCatType.TVAnime, "Animes 3D");

            AddCategoryMapping(3, TorznabCatType.PC0day, "APPLICATION");
            AddCategoryMapping(74, TorznabCatType.PCPhoneAndroid, "ANDROID");
            AddCategoryMapping(57, TorznabCatType.PCPhoneIOS, "IPHONE");
            AddCategoryMapping(6, TorznabCatType.PC0day, "LINUX");
            AddCategoryMapping(5, TorznabCatType.PCMac, "MAC");
            AddCategoryMapping(4, TorznabCatType.PC0day, "WINDOWS");

            AddCategoryMapping(38, TorznabCatType.TVDocumentary, "DOCUMENTAIRES");
            AddCategoryMapping(97, TorznabCatType.TVDocumentary, "DOCUMENTAIRES PACK");

            AddCategoryMapping(34, TorznabCatType.Books, "EBOOKS");
            AddCategoryMapping(86, TorznabCatType.Books, "ABOOKS");

            AddCategoryMapping(7, TorznabCatType.Movies, "FILMS");
            AddCategoryMapping(11, TorznabCatType.MoviesDVD, "DVD");
            AddCategoryMapping(10, TorznabCatType.MoviesSD, "DVD-RIP/BD-RIP");
            AddCategoryMapping(53, TorznabCatType.MoviesSD, "DVD-SCREENER");
            AddCategoryMapping(9, TorznabCatType.MoviesDVD, "R5");
            AddCategoryMapping(8, TorznabCatType.MoviesSD, "SCREENER");
            AddCategoryMapping(40, TorznabCatType.Movies, "VO");
            AddCategoryMapping(39, TorznabCatType.Movies, "VOSTFR");
            AddCategoryMapping(48, TorznabCatType.MoviesHD, "HD");
            AddCategoryMapping(51, TorznabCatType.MoviesHD, "1080P");
            AddCategoryMapping(70, TorznabCatType.Movies3D, "3D");
            AddCategoryMapping(50, TorznabCatType.MoviesHD, "720P");
            AddCategoryMapping(84, TorznabCatType.MoviesUHD, "4K");
            AddCategoryMapping(49, TorznabCatType.MoviesBluRay, "BluRay");
            AddCategoryMapping(78, TorznabCatType.MoviesHD, "HDLight");
            AddCategoryMapping(85, TorznabCatType.MoviesHD, "x265");
            AddCategoryMapping(91, TorznabCatType.Movies3D, "3D");
            AddCategoryMapping(95, TorznabCatType.Movies, "VOSTFR");

            AddCategoryMapping(15, TorznabCatType.Console, "JEUX VIDEO");
            AddCategoryMapping(76, TorznabCatType.Console3DS, "3DS");
            AddCategoryMapping(18, TorznabCatType.ConsoleNDS, "DS");
            AddCategoryMapping(55, TorznabCatType.PCPhoneIOS, "IPHONE");
            AddCategoryMapping(80, TorznabCatType.PCGames, "LINUX");
            AddCategoryMapping(79, TorznabCatType.PCMac, "OSX");
            AddCategoryMapping(22, TorznabCatType.PCGames, "PC");
            AddCategoryMapping(66, TorznabCatType.ConsolePS3, "PS2");
            AddCategoryMapping(58, TorznabCatType.ConsolePS3, "PS3");
            AddCategoryMapping(81, TorznabCatType.ConsolePS4, "PS4");
            AddCategoryMapping(20, TorznabCatType.ConsolePSP, "PSP");
            AddCategoryMapping(75, TorznabCatType.ConsolePS3, "PSX");
            AddCategoryMapping(19, TorznabCatType.ConsoleWii, "WII");
            AddCategoryMapping(83, TorznabCatType.ConsoleWiiU, "WiiU");
            AddCategoryMapping(16, TorznabCatType.ConsoleXbox, "XBOX");
            AddCategoryMapping(82, TorznabCatType.ConsoleXboxOne, "XBOX ONE");
            AddCategoryMapping(17, TorznabCatType.ConsoleXbox360, "XBOX360");
            AddCategoryMapping(44, TorznabCatType.ConsoleXbox360, "XBOX360.E");
            AddCategoryMapping(54, TorznabCatType.ConsoleXbox360, "XBOX360.JTAG");
            AddCategoryMapping(43, TorznabCatType.ConsoleXbox360, "XBOX360.NTSC");
            AddCategoryMapping(96, TorznabCatType.Console, "NSW");

            AddCategoryMapping(23, TorznabCatType.Audio, "MUSIQUES");
            AddCategoryMapping(26, TorznabCatType.Audio, "CLIP/CONCERT");
            AddCategoryMapping(61, TorznabCatType.AudioLossless, "FLAC");
            AddCategoryMapping(60, TorznabCatType.AudioMP3, "MP3");

            AddCategoryMapping(30, TorznabCatType.TV, "SERIES");
            AddCategoryMapping(73, TorznabCatType.TVSD, "Series Pack FR SD");
            AddCategoryMapping(92, TorznabCatType.TVHD, "Series Pack FR HD");
            AddCategoryMapping(93, TorznabCatType.TVSD, "Series Pack VOSTFR SD");
            AddCategoryMapping(94, TorznabCatType.TVHD, "Series Pack VOSTFR HD");
            AddCategoryMapping(31, TorznabCatType.TVSD, "Series FR SD");
            AddCategoryMapping(32, TorznabCatType.TVSD, "Series VO SD");
            AddCategoryMapping(33, TorznabCatType.TVSD, "Series VOSTFR SD");
            AddCategoryMapping(77, TorznabCatType.TVSD, "Series DVD");
            AddCategoryMapping(67, TorznabCatType.TVHD, "Series.FR HD");
            AddCategoryMapping(68, TorznabCatType.TVHD, "Series VO HD");
            AddCategoryMapping(69, TorznabCatType.TVHD, "Series VOSTFR HD");

            AddCategoryMapping(47, TorznabCatType.TV, "SPECTACLES/EMISSIONS");
            AddCategoryMapping(71, TorznabCatType.TV, "Emissions");
            AddCategoryMapping(72, TorznabCatType.TV, "Spectacles");

            AddCategoryMapping(35, TorznabCatType.TVSport, "SPORT");
            AddCategoryMapping(36, TorznabCatType.TVSport, "CATCH");
            AddCategoryMapping(65, TorznabCatType.TVSport, "UFC");

            AddCategoryMapping(37, TorznabCatType.XXX, "XXX");
        }

        public override async Task<IndexerConfigurationStatus> ApplyConfiguration(JToken configJson)
        {
            configData.LoadValuesFromJson(configJson);

            var pairs = new Dictionary<string, string>
            {
                { "username", configData.Username.Value },
                { "password", configData.Password.Value }
            };

            var result = await PostDataWithCookies(LoginUrl, pairs, "");

            await ConfigureIfOK(result.Cookies, result.Cookies != null, () =>
           {
               var errorMessage = result.Content;
               throw new ExceptionWithConfigData(errorMessage, configData);
           });

            return IndexerConfigurationStatus.RequiresTesting;
        }

        protected override async Task<IEnumerable<ReleaseInfo>> PerformQuery(TorznabQuery query)
        {
            var releases = new List<ReleaseInfo>();
            var searchString = query.GetQueryString();

            var queryCollection = new Dictionary<string, string>();
            queryCollection.Add("search_type", "t_name");
            queryCollection.Add("do", "search");
            queryCollection.Add("keywords", searchString);
            queryCollection.Add("category", "0"); // multi cat search not supported

            var results = await PostDataWithCookies(BrowseUrl, queryCollection);
            if (results.IsRedirect)
            {
                // re-login
                await ApplyConfiguration(null);
                results = await PostDataWithCookies(BrowseUrl, queryCollection);
            }

            try
            {
                var RowsSelector = "table[id='sortabletable'] > tbody > tr";
                var SearchResultParser = new HtmlParser();
                var SearchResultDocument = SearchResultParser.ParseDocument(results.Content);
                var Rows = SearchResultDocument.QuerySelectorAll(RowsSelector);
                var lastDate = DateTime.Now;

                foreach (var Row in Rows.Skip(1))
                {
                    var release = new ReleaseInfo();
                    release.MinimumRatio = 1;
                    release.MinimumSeedTime = 0;

                    var category = Row.QuerySelector("td:nth-child(1) > a");
                    var title = Row.QuerySelector("td:nth-child(2) a");
                    var added = Row.QuerySelector("td:nth-child(2) > div:has(span[style=\"float: right;\"])");
                    if (added == null) // not a torrent line
                        continue;
                    var pretime = added.QuerySelector("font.mkprettytime");
                    var tooltip = Row.QuerySelector("td:nth-child(2) > div.tooltip-content");

                    var link = Row.QuerySelector("td:nth-child(3)").QuerySelector("a");
                    var comments = Row.QuerySelector("td:nth-child(2)").QuerySelector("a");
                    var Size = Row.QuerySelector("td:nth-child(5)");
                    var Grabs = Row.QuerySelector("td:nth-child(6)").QuerySelector("a");
                    var Seeders = Row.QuerySelector("td:nth-child(7)").QuerySelector("a");
                    var Leechers = Row.QuerySelector("td:nth-child(8)").QuerySelector("a");

                    var categoryId = category.GetAttribute("href").Split('=')[1];
                    release.Category = MapTrackerCatToNewznab(categoryId);

                    release.Title = title.TextContent;
                    release.Category = MapTrackerCatToNewznab(categoryId);
                    release.Link = new Uri(link.GetAttribute("href"));
                    release.Comments = new Uri(comments.GetAttribute("href"));
                    release.Guid = release.Link;
                    release.Size = ReleaseInfo.GetBytes(Size.TextContent);
                    release.Seeders = ParseUtil.CoerceInt(Seeders.TextContent);
                    release.Peers = ParseUtil.CoerceInt(Leechers.TextContent) + release.Seeders;
                    release.Grabs = ParseUtil.CoerceLong(Grabs.TextContent);

                    if (TorrentHTTPSMode)
                    {
                        var linkHttps = Row.QuerySelector("td:nth-child(4)").QuerySelector("a").GetAttribute("href");
                        var idTorrent = ParseUtil.GetArgumentFromQueryString(linkHttps, "id");
                        release.Link = new Uri($"{SiteLink}download.php?id={idTorrent}&type=ssl");
                    }

                    if (added.QuerySelector("img[alt^=\"TORRENT GRATUIT\"]") != null)
                        release.DownloadVolumeFactor = 0;
                    else if (added.QuerySelector("img[alt^=\"TORRENT SILVER\"]") != null)
                        release.DownloadVolumeFactor = 0.5;
                    else
                        release.DownloadVolumeFactor = 1;

                    if (added.QuerySelector("img[alt^=\"TORRENT X2\"]") != null)
                        release.UploadVolumeFactor = 2;
                    else
                        release.UploadVolumeFactor = 1;

                    if (tooltip != null)
                    {
                        var banner = tooltip.QuerySelector("img");
                        if (banner != null)
                        {
                            release.BannerUrl = new Uri(banner.GetAttribute("src"));
                            banner.Remove();
                        }

                        tooltip.QuerySelector("div:contains(\"Total Hits\")").Remove();

                        var longtitle = tooltip.QuerySelectorAll("div").First();
                        release.Title = longtitle.TextContent;
                        longtitle.Remove();

                        var desc = tooltip.TextContent.Trim();
                        if (!string.IsNullOrWhiteSpace(desc))
                            release.Description = desc;
                    }

                    //issue #5064 replace multi keyword
                    if (!string.IsNullOrEmpty(ReplaceMulti))
                    {
                        var regex = new Regex("(?i)([\\.\\- ])MULTI([\\.\\- ])");
                        release.Title = regex.Replace(release.Title, "$1" + ReplaceMulti + "$2");
                    }
                    // issue #6855 Replace VOSTFR with ENGLISH
                    if (configData.Vostfr.Value)
                    {
                        release.Title = release.Title.Replace("VOSTFR", "ENGLISH");
                    }

                    if (pretime != null)
                    {
                        if (release.Description == null)
                            release.Description = pretime.TextContent;
                        else
                            release.Description += "<br>\n" + pretime.TextContent;
                        release.PublishDate = lastDate;
                    }
                    else
                    {
                        release.PublishDate = DateTime.ParseExact(added.TextContent.Trim(), "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                        lastDate = release.PublishDate;
                    }

                    releases.Add(release);
                }
            }
            catch (Exception ex)
            {
                OnParseError(results.Content, ex);
            }

            return releases;
        }
    }
}
