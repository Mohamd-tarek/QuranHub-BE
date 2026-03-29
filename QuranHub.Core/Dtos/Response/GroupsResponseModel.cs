using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranHub.Core.Dtos.Response
{
    public class GroupsResponseModel
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public List<VerseResponseModel> Verses { get; set; }
    }
}
