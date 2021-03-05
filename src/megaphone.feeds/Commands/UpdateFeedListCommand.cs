﻿using System.Threading.Tasks;
using System.Collections.Generic;
using Megaphone.Feeds.Services;
using Megaphone.Feeds.Models;
using Feeds.API.Commands;
using Megaphone.Standard.Commands;
using Megaphone.Standard.Services;
using Megaphone.Feeds.Queries;
using System;
using System.Diagnostics;

namespace Megaphone.Feeds.Commands
{
    internal class UpdateFeedListCommand : ICommand<IPartionedStorageService<StorageEntry<List<Feed>>>>
    {
        readonly Feed feed;

        public UpdateFeedListCommand(Feed feed)
        {
            this.feed = feed;
        }

        public async Task ApplyAsync(IPartionedStorageService<StorageEntry<List<Feed>>> model)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(model);

            var i = entry.Value.Find(i => i.Id == feed.Id);
            if (IsNotDefault(i))
            {
                i.LastCrawled = feed.LastCrawled;
                i.LastHttpStatus = feed.LastHttpStatus;
                i.Display = feed.Display;

                var c = new PersistFeedListCommand(entry);
                await c.ApplyAsync(model);

                if (Debugger.IsAttached)
                    Console.WriteLine($"[] | feed update : \"{i.Display}\" ({i.LastCrawled.ToString("s")})");
            }
        }

        static bool IsNotDefault(Feed f)
        {
            return f != null && !string.IsNullOrEmpty(f.Id);
        }
    }
}