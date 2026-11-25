param(
  [string[]] $Api = @("http://localhost:5080"),  # API ตัวเดียวหรือหลายตัว
  [int] $Count = 100                              # จำนวน request ต่อ API หนึ่งตัว
)
$dir = Join-Path $env:TEMP "queue-burst"
New-Item -ItemType Directory -Force $dir | Out-Null
Remove-Item "$dir\*" -ErrorAction SilentlyContinue

curl.exe -s -X POST "$($Api[0])/api/queue/reset" | Out-Null   # เริ่มจาก 00

# รวมทุก request ไว้ในคำสั่ง curl เดียว --parallel จะส่งออกไปพร้อมกันทั้งหมด
$curlArgs = @("-s", "--parallel", "--parallel-immediate", "--parallel-max", "300", "-X", "POST", "-w", "%{http_code}\n")
$i = 0
foreach ($a in $Api) {
  $i++
  $curlArgs += @("${a}/api/queue/tickets?n=[1-$Count]", "-o", "$dir\api${i}_#1.json")
}
$statuses = & curl.exe @curlArgs

$statuses | Group-Object | Select-Object @{n='Status';e={$_.Name}}, Count | Format-Table -AutoSize
$codes = Get-ChildItem $dir -Filter *.json | ForEach-Object { (Get-Content $_.FullName -Raw | ConvertFrom-Json).code }
$sorted = $codes | Sort-Object
"tickets: $($codes.Count)   unique codes: $(($codes | Sort-Object -Unique).Count)   first: $($sorted[0])   last: $($sorted[-1])"