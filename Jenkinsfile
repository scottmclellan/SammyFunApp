pipeline {
agent { node {label 'MSBuild'} }
  stages {
    stage('Build') {
      steps {
        echo 'Building..'
         bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\" /t:Build /p:Configuration=Release /p:TargetFrameworkVersion=v4.0"
      }
    }   
    stage('Emailing') {
      steps {
        echo 'Deploying....'
      }
    }
  }
}