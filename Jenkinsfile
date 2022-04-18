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
          bat "\"C:\\Program Files\\7-Zip\\7z.exe\" a ${env.JOB_NAME}.${env.BUILD_ID}.zip"
        }
      }
    }   
    stage('Emailing') {
      steps {
        echo 'Emailing....'
        dir('SammyFunApp\\bin\\Release\\'){
        bat "xcopy ${env.JOB_NAME}.${env.BUILD_ID}.zip C:\\Jenkins\\Artifacts"
        }
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