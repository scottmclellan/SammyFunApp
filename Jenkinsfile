pipeline {
agent { node {label 'MSBuild_NETFramework'} }
  stages {
    stage('Build') {
      steps {
        echo 'Building..'         
         bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\" /t:Build /restore /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.2"
      }
    }   
    stage('Emailing') {
      steps {
        echo 'Emailing....'
      }
    }
  }
}