import { export_json_to_excel } from "@/excel/Export2Excel";
function formatJson(filterVal, jsonData) {
  return jsonData.map(v => filterVal.map(j => v[j]));
}

function export2Excel(fileName, tHeader, filterVal, excelData) {
  const data = formatJson(filterVal, excelData);
  export_json_to_excel(tHeader, data, fileName);
}

/*tHeader_导出的表头名;
const tHeader = [
  "测试用例编号",
  "本月的通话分钟数X（分钟）",
  "本年度至本月的累计未按时缴费的次数Y（次）",
  "每月的电话总费用预期输出",
]
*/

/*filterVal_导出的表头字段名;
const filterVal = [
      "userId",
      "name",
      "age",
      "status",
];
*/

/*excelData_导出的数据;
const excelData= [{userId:1,name:'小白',age:'18',status:"上学"},
{userId:2,name:'小黑',age:'22',status:"待业"},
{userId:3,name:'小红',age:'28',status:"就业"}]
*/


function printExe(fileName, tHeader, filterVal, excelData) {
  console.log('printExe executed');
  export2Excel(fileName, tHeader, filterVal, excelData);
}

export default printExe