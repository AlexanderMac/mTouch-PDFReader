#!/bin/bash

version_number=1
for value in `cat version.txt`
do
	version_number=`expr $value + 1`
	break
done

echo $version_number
echo $version_number > 'version.txt'
