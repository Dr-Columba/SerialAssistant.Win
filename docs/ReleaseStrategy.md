# Release Strategy

## Overview

This document defines the release strategy for SerialAssistant.Win, focusing on framework-dependent deployment and minimal runtime dependencies.

## 1. Deployment Approach

### Framework-Dependent Deployment (Recommended)

**Strategy:**
- Deploy only application binaries
- Depend on .NET Desktop Runtime installed on target machine
- Use standard .NET apphost for execution

**Benefits:**
- Small deployment package (~1-2 MB)
- Faster startup (no self-contained overhead)
- Easier updates (smaller download size)
- Better compatibility with system .NET installations

**Drawbacks:**
- Requires .NET Desktop Runtime on target machine
- First-time users may need to install runtime

### Not Recommended Approaches

| Approach | Reason |
|----------|--------|
| Self-contained deployment | Large package size, unnecessary duplication |
| PublishSingleFile | Compression overhead, slower startup |
| Trimmed deployment | Risk of breaking reflection-based features |
| ReadyToRun | Increased disk footprint, minimal performance gain |
| NativeAOT | Complex deployment, potential compatibility issues |

## 2. Runtime Requirements

### Required Runtime
- **.NET Desktop Runtime 8.0** or later
- **Windows 10 1809+** or **Windows 11**

### Installation Sources

**Official Download:**
- https://dotnet.microsoft.com/download/dotnet/8.0
- Select ".NET Desktop Runtime"

**Silent Installation (for IT deployment):**
```powershell
dotnet-runtime-8.0.x-win-x64.exe /quiet /norestart
```

## 3. Build Commands

### Development Build
```powershell
dotnet build .\SerialAssistant.Win.sln -c Debug
```

### Release Build
```powershell
dotnet build .\SerialAssistant.Win.sln -c Release
```

### Publish for Deployment
```powershell
dotnet publish .\src\SerialAssistant.App\SerialAssistant.App.csproj `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -o .\publish\win-x64
```

### Clean Publish
```powershell
dotnet publish .\src\SerialAssistant.App\SerialAssistant.App.csproj `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -o .\publish\win-x64 `
    /p:UseAppHost=true
```

## 4. Output Structure

```
publish/
├── win-x64/
│   ├── SerialAssistant.App.exe          # Main executable
│   ├── SerialAssistant.App.dll          # Main application assembly
│   ├── SerialAssistant.Core.dll         # Core library
│   ├── SerialAssistant.Infrastructure.dll  # Infrastructure layer
│   ├── System.Text.Json.dll             # JSON serialization
│   └── (other .NET runtime dependencies)
```

## 5. Runtime Detection

### Current Approach (Short-term)

**Apphost Behavior:**
- When .NET runtime is missing, apphost displays default error message
- Error message includes link to download .NET runtime
- User must manually install runtime

**Error Message Example:**
```
You must install .NET to run this application.

App: C:\Path\To\SerialAssistant.App.exe
Architecture: x64
Framework: 'Microsoft.NETCore.App', version '8.0.0' (x64)

Learn more:
https://aka.ms/dotnet/app-launch-failed

Download the .NET runtime:
https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=8.0.0&arch=x64&rid=win-x64
```

### Future Enhancement (Native Launcher)

**Proposed Solution:**
- Create lightweight native launcher (C/C++)
- Detects .NET runtime presence
- Shows friendly dialog with download button
- Downloads and installs runtime if missing
- Launches main application after installation

**Benefits:**
- Better user experience
- One-click installation
- No command-line interaction needed

**Current Status:** Not implemented, planned for future phase

## 6. Versioning Strategy

### Semantic Versioning

```
v{Major}.{Minor}.{Patch}[-{PreRelease}]
```

| Component | Meaning |
|-----------|---------|
| Major | Breaking changes, major new features |
| Minor | New features, backward compatible |
| Patch | Bug fixes, minor improvements |
| PreRelease | Alpha/beta/rc versions |

### Version History

| Version | Date | Description |
|---------|------|-------------|
| v0.1.0 | May 2026 | Initial release (Features A-B) |
| v0.2.0 | May 2026 | Features C-D completed |

### Tagging Convention

```
git tag -a vX.Y.Z -m "vX.Y.Z - Description"
git push origin vX.Y.Z
```

## 7. Update Strategy

### Manual Updates (Current)
- User downloads new version from repository
- Extracts to installation directory
- Overwrites existing files

### Future Enhancement (Auto-update)
- Check for updates on startup
- Download updates in background
- Prompt user to restart
- Install updates on exit

**Requirements:**
- GitHub API integration
- Update package verification
- Rollback capability

**Current Status:** Not implemented

## 8. Distribution Channels

### Primary Channel
- GitHub Releases: https://github.com/[repo]/releases

### Alternative Channels
- Chocolatey package (future)
- WinGet package (future)
- Manual download from repository

## 9. Security Considerations

### Code Signing
- Code signing certificate recommended for production
- Prevents tampering detection warnings
- Build server should have access to signing certificate

### Update Integrity
- Verify download checksums
- Use signed packages
- Validate updates before installation

### Permissions
- Requires standard user permissions
- No admin rights needed for installation
- Configuration stored in user AppData

## 10. Build Configuration

### csproj Settings

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Platforms>x64</Platforms>
    <PublishReadyToRun>false</PublishReadyToRun>
    <PublishSingleFile>false</PublishSingleFile>
    <SelfContained>false</SelfContained>
    <UseAppHost>true</UseAppHost>
  </PropertyGroup>
</Project>
```

### Directory.Build.props

```xml
<Project>
  <PropertyGroup>
    <VersionPrefix>0.2.0</VersionPrefix>
    <Authors>SerialAssistant.Win Team</Authors>
    <Company>SerialAssistant.Win</Company>
    <Product>SerialAssistant.Win</Product>
    <Copyright>Copyright (c) 2026</Copyright>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>
</Project>
```

## 11. CI/CD Recommendations

### GitHub Actions Workflow

**Trigger:**
- Push to main branch
- Pull requests
- Manual dispatch

**Steps:**
1. Checkout repository
2. Setup .NET 8 SDK
3. Build solution
4. Run tests
5. Publish application
6. Create release (on tag)

**Example Workflow:**
```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Build
      run: dotnet build -c Release
    
    - name: Test
      run: dotnet test -c Release --no-build
    
    - name: Publish
      run: dotnet publish .\src\SerialAssistant.App\SerialAssistant.App.csproj -c Release -r win-x64 --self-contained false -o .\publish
    
    - name: Create Release
      uses: softprops/action-gh-release@v2
      with:
        files: |
          publish/**/*
```

## 12. Release Checklist

### Before Release
- [ ] All tests pass
- [ ] Build succeeds without warnings
- [ ] Version number updated
- [ ] Release notes written
- [ ] Documentation updated

### During Release
- [ ] Create git tag
- [ ] Push tag to origin
- [ ] Run CI/CD workflow
- [ ] Verify published artifacts
- [ ] Create GitHub Release

### After Release
- [ ] Test installation on clean machine
- [ ] Verify runtime detection works
- [ ] Update changelog
- [ ] Announce release

---

*Last updated: May 2026*
