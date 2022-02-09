pipeline {
agent { node {label 'MSBuild_NETFramework'} }
  stages {
    stage('Build') {
      steps {
         echo 'Restoring..'         
         bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\" /t:Restore"

        echo 'Building..'         
         bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\" /t:Build /p:Configuration=Release"
      }
    }   
    stage('Emailing') {
      steps {
        echo 'Emailing....'
      }
    }
  }
}