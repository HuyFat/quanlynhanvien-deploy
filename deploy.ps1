#!/usr/bin/env pwsh

# Deploy script for QuanLyNhanVien to Railway
# This script automates the GitHub push process

param(
    [string]$CommitMessage = "Update: deployment",
    [switch]$FirstTime = $false
)

# Colors for output
$Green = "`e[32m"
$Red = "`e[31m"
$Yellow = "`e[33m"
$Reset = "`e[0m"

Write-Host "${Green}=== QuanLyNhanVien Deploy Script ===${Reset}"
Write-Host ""

# Check if git is installed
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "${Red}❌ Git is not installed. Please install Git from https://git-scm.com${Reset}"
    exit 1
}

# Get current directory
$RepoPath = Split-Path -Parent $MyInvocation.MyCommandPath
if (-not (Test-Path "$RepoPath\.git")) {
    Write-Host "${Yellow}⚠️  This doesn't appear to be a git repository.${Reset}"
    Write-Host "${Yellow}Initializing git repository...${Reset}"
    
    # Initialize git
    git init
    git config user.name "Developer"
    git config user.email "dev@example.com"
    
    Write-Host "${Green}✅ Git repository initialized${Reset}"
    Write-Host ""
    Write-Host "${Yellow}Next steps:${Reset}"
    Write-Host "1. Create a new repository on GitHub: https://github.com/new"
    Write-Host "2. Run this command with the GitHub URL:"
    Write-Host "   git remote add origin <your-github-url>"
    Write-Host "3. Run this script again to push to GitHub"
    Write-Host ""
    exit 0
}

# Check if remote exists
$RemoteOrigin = git config --get remote.origin.url
if ([string]::IsNullOrEmpty($RemoteOrigin)) {
    Write-Host "${Red}❌ Git remote 'origin' not configured${Reset}"
    Write-Host "${Yellow}Set up the remote with:${Reset}"
    Write-Host "  git remote add origin <your-github-url>"
    Write-Host ""
    Write-Host "${Yellow}Then run this script again.${Reset}"
    exit 1
}

Write-Host "${Green}📁 Repository:${Reset} $RepoPath"
Write-Host "${Green}📍 Remote:${Reset} $RemoteOrigin"
Write-Host ""

# Build and publish
Write-Host "${Yellow}🔨 Building application...${Reset}"
Push-Location "$RepoPath\QuanLyNhanVien"
$BuildResult = dotnet build -c Release 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "${Red}❌ Build failed. Fix errors and try again.${Reset}"
    Pop-Location
    exit 1
}
Pop-Location
Write-Host "${Green}✅ Build successful${Reset}"
Write-Host ""

# Stage changes
Write-Host "${Yellow}📝 Staging changes...${Reset}"
git add .
Write-Host "${Green}✅ Changes staged${Reset}"
Write-Host ""

# Commit
Write-Host "${Yellow}💾 Committing changes...${Reset}"
git commit -m $CommitMessage
if ($LASTEXITCODE -ne 0) {
    Write-Host "${Yellow}⚠️  No changes to commit${Reset}"
} else {
    Write-Host "${Green}✅ Changes committed${Reset}"
}
Write-Host ""

# Push to GitHub
Write-Host "${Yellow}🚀 Pushing to GitHub...${Reset}"
git push -u origin main
if ($LASTEXITCODE -eq 0) {
    Write-Host "${Green}✅ Push successful!${Reset}"
} else {
    Write-Host "${Red}❌ Push failed. Check your GitHub URL and credentials.${Reset}"
    exit 1
}

Write-Host ""
Write-Host "${Green}=== Deployment Pushed to GitHub ===${Reset}"
Write-Host ""
Write-Host "${Green}📱 Next Steps:${Reset}"
Write-Host "1. Go to Railway: https://railway.app"
Write-Host "2. Create new project from GitHub repo"
Write-Host "3. Set ConnectionStrings__DefaultConnection environment variable"
Write-Host "4. Deployment will start automatically"
Write-Host "5. Once live, access at the URL provided by Railway from any device!"
Write-Host ""
