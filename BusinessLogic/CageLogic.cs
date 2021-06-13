using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;

namespace RabbitFarmLocal.BusinessLogic
{
    public  class CageLogic
    {
       public static string CageJSON (int higlightedCage, bool oneRow)
        {
            List<CageModel> cages = Cage.LoadAll().OrderBy(x => x.Row).ThenBy(y => y.PositionInRow).ThenBy(z => z.Level).ToList();
            if (oneRow)
            {
                int? CageRow = cages.Find(x => x.Id == higlightedCage).Row;
                if(CageRow!=null) cages.RemoveAll(x => x.Row != (int)CageRow);

            }
            int? row = null, pos = null, lev = null;
            int? r = null, p = null;//to track changes
            CageAssumptions ca = new();

            foreach (var c in cages)
            {
                if (row == null)
                {
                    r = c.Row;
                    if (row == null) row = 1;
                    else row++;
                    ca.Rows = new List<CageRow>
                    {
                        new CageRow
                        {
                        Nr = (int) r
                        }
};
                    pos = null;
                }
                else if (c.Row != r)
                {
                    r = c.Row;
                    row++;
                    p = c.Row;
                    ca.Rows.Add(new CageRow
                    {
                        Nr = (int)r
                    });
                    pos = null;
                }
                if (pos == null)
                {
                    p = c.PositionInRow;
                    pos = 1;
                    ca.Rows.Last().Frames = new List<CageFrame>
                    {
                        new CageFrame
                        {
                            Nr = (int)pos
                        }
                    };
                    lev = null;
                }
                else if (c.PositionInRow != p)
                {
                    pos++;
                    p = c.PositionInRow;
                    ca.Rows.Last().Frames.Add(new CageFrame
                    {
                        Nr = (int)pos
                    });
                    lev = null;
                }
                if (lev == null)
                {
                    ca.Rows.Last().Frames.Last().Levels = new List<CageLevel>();
                    lev = 1;
                }
                ca.Rows.Last().Frames.Last().Levels.Add(new CageLevel
                {
                    Nr = (int)lev,
                    Id = c.Id
                });

                lev++;


            }
            // ViewBag.CA = ca;
            string caJson="";
            try
            {
                caJson = JsonSerializer.Serialize<CageAssumptions>(ca);
                
            }
            catch (NotSupportedException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            };
            return caJson;
        }


    }
}
