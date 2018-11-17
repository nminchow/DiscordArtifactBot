﻿using Artifact.Models;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks; 

namespace Artifact.Controllers.Card
{
    class TextScan
    {
        public static async Task PerformAsync(SocketCommandContext context, DataBase db)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\[\] ]");
            var guild = Guild.FindOrCreate.Perform(context.Guild, db);
            var message = regex.Replace(context.Message.Content.ToLower(), "");
            var hits = new List<Models.Card>();

            switch (guild.LookupSetting)
            {
                case LookupSetting.none:
                    break;
                case LookupSetting.all:
                    hits = LoadCards.Instance.cards.Where(x => message.Contains(replace(regex, x))).ToList();
                    break;
                case LookupSetting.brackets:
                    // this could be cleaner with some substring logic
                    hits = LoadCards.Instance.cards.Where(x => message.Contains($"[[{replace(regex, x)}]]")).ToList();
                    break;
            }
            await Helpers.SendCards.PerformAsync(context, hits, guild.DisplaySetting);

            await DeckLinks.PerformAsync(context, db);

            return;

        }

        public static string replace(Regex regex, Models.Card card)
        {
            return regex.Replace(card.card_name.english.ToLower(), "");
        }
    }
}
