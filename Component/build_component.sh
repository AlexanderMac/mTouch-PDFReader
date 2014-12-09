#!/bin/bash

rm -r ./lib
rm -r ./samples
rm -r ./screenshots
echo "Directores removed"

mkdir ./screenshots
mkdir ./lib
mkdir ./lib/ios
mkdir ./samples
mkdir ./samples/ios
echo "Directores created"

rsync ../screenshots/* ./screenshots
echo "Screenshots copied"

rsync ../library/bin/release/mTouchPDFReaderLibrary.dll ./lib/ios/
rsync ../library/bin/release/autofac.dll ./lib/ios/
echo "Library dlls copied"

rsync -r --exclude=bin --exclude=obj ../demo/* ./samples/ios 

echo "Run mono xamarin-component.exe package component"