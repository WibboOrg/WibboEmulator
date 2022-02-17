#!/bin/sh

docker build . -t --no-cache jasondhose/wibboemulator
docker run -p 444:444 -p 30001:30001 -p 527:527 jasondhose/wibboemulator