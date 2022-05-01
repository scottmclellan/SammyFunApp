pipeline {
agent { node {label 'MSBuild_NETFramework'} }
  stages {
    stage('Build') {
      steps {
         echo 'Restoring..'         
         bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\" /restore"

        echo 'Building..'         
         bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\" /t:Build /p:Configuration=Release"
      }
    }   
    stage('Zipping'){
      steps{
        echo 'Zipping'
        dir('SammyFunApp\\bin\\Release\\'){
          bat 'cd'
          bat "\"C:\\Program Files\\7-Zip\\7z.exe\" a SammyFunApp.${env.BUILD_ID}.zip"
        }
      }
    }   
    stage('Emailing') {
      steps {
        echo 'Emailing....'
        dir('SammyFunApp\\bin\\Release\\'){
        bat "xcopy SammyFunApp.${env.BUILD_ID}.zip C:\\Jenkins\\Artifacts"
        }
        dir('SetupProject\\bin\\Release\\'){
          bat "xcopy SammyPaintShopSetup.msi C:\\Jenkins\\Artifacts /Y"
        }
      }
    }
    stage('Creating Release')
    {
      steps{
        bat "echo %PATH%"
        bat "\"C:\\Program Files\\GitHub CLI\\gh.exe\" release create v1.0.${env.BUILD_ID} SetupProject\\bin\\Release\\SammyPaintShop.msi"
      }
    }
    stage('Cleaning Workspace')
    {
      steps{
      cleanWs()
      }
    }
  }
}