using SqlSugar;

namespace Meeteam_Backend
{
    [SugarTable("project_img")]
    public class Project_Img
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string project_id { get; set; }
        public string img_path { get; set; }
    }
}
