using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class NoteModel
    {
        [DisplayName("Создан")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DisplayName("Создан")]
       
        public string DateString { get { return DateToStringRU(Date); } }
        public List<NoteModelDate> DateList { get; set; } = new List<NoteModelDate>();
        [DisplayName("Заметка")]
        public string Note { get; set; }    
    }
    public class NoteModelDate
    {
        public  DateTime Date { get; set; }
        public  string DateString { get { return DateToStringRU(Date); } }
        public string DateStringLink { get { return DateToString(Date); } }
    }
}
