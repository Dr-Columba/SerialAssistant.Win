# Validation Gate

## 1. Purpose

This document defines the unified validation gate for every Phase in SerialAssistant.Win development. All feature phases must pass this gate before being considered complete.

## 2. Required Gate Commands

Every phase must execute or request user execution of the following commands:

```powershell
# Branch Verification
git branch --show-current
git status --short

# Build and Test
dotnet build .\SerialAssistant.Win.sln -c Debug
dotnet test .\SerialAssistant.Win.sln -c Debug

# Diff Check
git diff --check
echo $LASTEXITCODE

# Changed Files
git diff --name-only
git diff --name-status
```

## 3. Branch Rules

| Rule | Description |
|------|-------------|
| No direct main development | All features must use dedicated feature branches |
| Feature branch naming | Use format: feature/{feature-name} |
| main branch acceptance | Only accepts merges after validation gate passes |
| Pre-commit requirements | Working directory must be explainable |
| Post-commit requirements | Must push feature branch to remote |

## 4. Diff Check Rules

| Check | Expected Result |
|-------|-----------------|
| `git diff --check` | No output (0 lines) |
| `echo $LASTEXITCODE` | Must be 0 |
| Trailing whitespace | Any trailing whitespace is a failure |
| Markdown files | Must also pass diff check |

**Critical:** Even documentation files (`.md`) must pass `git diff --check`.

## 5. Build and Test Rules

| Check | Command | Expected |
|-------|---------|----------|
| Debug Build | `dotnet build -c Debug` | 0 errors, 0 warnings |
| Debug Test | `dotnet test -c Debug` | All tests pass |

### Release Build Requirement

If the phase modifies:
- Build configuration
- Project files (*.csproj)
- Solution files (*.sln)
- Runtime policies
- Publish settings

Then also execute:

```powershell
dotnet build .\SerialAssistant.Win.sln -c Release
```

**Note:** Release tests are not required for every phase unless explicitly mentioned above.

## 6. Changed File Scope Rules

### Documentation Phase Scope
- Allowed: README.md, docs/*.md, docs/FeatureReports/*.md
- Forbidden: src/, tests/, *.csproj, *.sln

### Feature Phase Scope
- Must document all modified src files
- Must document all modified test files
- Must not delete functionality to pass tests
- Must not delete tests to pass validation

### Forbidden File Types
Do not commit:
- bin/ directories
- obj/ directories
- .vs/ directories
- TestResults/ directories
- Temporary files
- Cache files

## 7. Architecture Boundary Checks

### Layer Dependency Rules

| Layer | Forbidden References |
|-------|----------------------|
| App | System.IO.Ports (direct) |
| Core | WPF, System.IO.Ports, Filesystem |
| Infrastructure | WPF (Dispatcher, Window, etc.) |

### PowerShell Verification Commands

**App Layer Check:**
```powershell
Select-String -Path .\src\SerialAssistant.App\**\*.cs -Pattern "System.IO.Ports"
```

**Core Layer Check:**
```powershell
Select-String -Path .\src\SerialAssistant.Core\**\*.cs -Pattern "System.IO.Ports","System.Windows","File.","Directory.","JsonSerializer","Registry"
```

**Infrastructure Layer Check:**
```powershell
Select-String -Path .\src\SerialAssistant.Infrastructure\**\*.cs -Pattern "System.Windows","Window","Dispatcher"
```

### Interpretation
- If any command returns results, verify if it's a violation
- Explain any found patterns in the Phase Report

## 8. UI Boundary Checks

### MainWindow.xaml.cs Rules
- Must remain minimal
- No business logic
- Only InitializeComponent and window lifecycle events

### Verification Commands

**MainWindow.xaml.cs Content Check:**
```powershell
Select-String -Path .\src\SerialAssistant.App\MainWindow.xaml.cs -Pattern "Open","Close","Send","Receive","Write","Read","SerialPort","File","Directory","JsonSerializer"
```

**XAML Event Binding Check:**
```powershell
Select-String -Path .\src\SerialAssistant.App\MainWindow.xaml -Pattern '\s(Click|Loaded|SelectionChanged|TextChanged|Checked|Unchecked)="'
```

### UI Development Guidelines
- Prefer Binding, Command, ViewModel over code-behind events
- If events are necessary, document the reason in the Phase Report

## 9. Comment Style Checks

### C# Comment Rules
- New C# comments must NOT use double-slash style (//)
- Use block comments (/* */) instead

### Verification Command
```powershell
Select-String -Path .\src\SerialAssistant.App\**\*.cs,.\src\SerialAssistant.Core\**\*.cs,.\src\SerialAssistant.Infrastructure\**\*.cs,.\src\SerialAssistant.Tests\**\*.cs -Pattern "//"
```

### Exceptions
- URLs in comments (e.g., https://) are allowed
- Must document any intentional double-slash comments

## 10. Documentation Keyword Checks

### Feature-Specific Keywords
Adjust keywords for each feature phase.

**Example (Feature D - Send History):**
```powershell
Select-String -Path .\README.md,.\docs\Architecture.md,.\docs\ManualTestChecklist.md,.\docs\FinalReview.md,.\docs\FeatureReports\*.md -Pattern "SendHistory","SendHistoryItem","MaxSendHistoryCount","SelectedSendHistoryItem","发送历史","清空历史","最新","去重","持久化"
```

### Documentation Quality Checks
- No incorrect version numbers
- No incorrect branch names
- No incorrect tags
- No leftover content from previous features

## 11. Mojibake Check

### Python Verification Script
```powershell
@'
from pathlib import Path

bad_tokens = [
    "\u6d93",
    "\u934f",
    "\u95bf",
    "\u20ac",
    "\ufffd",
    "\u93c3",
    "\u9286",
]

paths = list(Path("src").rglob("*.cs")) + list(Path("docs").rglob("*.md")) + [Path("README.md")]
found = False

for path in paths:
    if path.exists():
        text = path.read_text(encoding="utf-8")
        for token in bad_tokens:
            if token in text:
                print(f"FOUND_BAD_TOKEN: {path}: {token!r} U+{ord(token):04X}")
                found = True

if not found:
    print("OK: no common mojibake tokens found")
'@ | python
```

### Rules
- FOUND_BAD_TOKEN must be treated as validation failure
- Do not ignore Chinese encoding issues
- Do not delete content to avoid detection

## 12. Third-Party Dependency Checks

### Project File Verification
```powershell
git diff main..HEAD -- *.csproj
git diff main..HEAD -- *.sln
```

### Rules
- No new third-party libraries
- No changes to TargetFramework without documentation
- No changes to project references without justification

## 13. Phase Report Rules

### Required Report Location
`docs/FeatureReports/Feature{PhaseName}-{Description}.md`

### Required Report Sections
1. Phase Summary
2. Modified Files
3. Scope Control
4. Architecture Boundary Review
5. Validation Results
6. User Verification Commands
7. Final Recommendation

### Report Template
```markdown
# Phase Report

## 1. Phase Information
Phase:
Goal:
Execution Result:

## 2. Current Git Status
Current Branch:
Git Status:
Latest Commit:

## 3. Modified Files
Added:
Modified:
Deleted:

## 4. Implementation Content
Completed:
Not Completed:

## 5. Boundary Check
Modified Source Code?:
Modified Tests?:
Added New Features?:
Auto Commit?:
Auto Push?:

## 6. Validation Results
[All validation command outputs]

## 7. User Verification Commands
[Complete PowerShell commands]

## 8. Known Issues
[Or "暂无已知问题"]

## 9. Next Step Recommendations
Suggest Merge?:
Suggested Commit Message:
```

## 14. Manual Verification Rules

### UI/Interaction Phases
Must update `docs/ManualTestChecklist.md` with new test steps.

### Documentation-Only Phases
May skip ManualTestChecklist.md update if:
- No UI changes
- No user interaction changes
- Must document this decision in Phase Report

## 15. Acceptance Rules

### Prerequisites for Accept
- [ ] `git diff --check` passes (no output)
- [ ] Build passes (0 errors)
- [ ] Tests pass (all green)
- [ ] Changed scope matches phase constraints
- [ ] Phase Report created/updated
- [ ] User local verification completed
- [ ] ChatGPT determines Accept

### Rejection Criteria
- Any trailing whitespace
- Build errors
- Test failures
- Out-of-scope file changes
- Report not created/updated
- User verification not completed

---

*Last updated: May 2026*
