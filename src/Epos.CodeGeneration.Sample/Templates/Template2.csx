Output(
$@"using Epos.CodeGeneration.Sample;

namespace TestData
{{
    public static class Drivers
    {{"
);

int i = 1;
foreach (var theDriver in Parameter) {
    Output(
$@"
        public static Driver Driver{i++} = new Driver {{
            Name = ""{theDriver.Name}"",
            NumberOfChampionships = {theDriver.NumberOfChampionships},
            Team = ""{theDriver.Team}""
        }};
"
    );
}

Output(
$@"
    }}
}}"
);
