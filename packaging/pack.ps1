$project = Join-Path $PsScriptRoot "..\libraries\Pliant\Pliant.csproj"
$project = Resolve-Path $project
nuget pack $project -Prop Configuration=Release