#!/bin/sh
cd bin/Debug/net7.0

gnuplot << EOF
set grid
set terminal pngcairo size 1920,1080 enhanced
set xrange [0:2000]
set datafile separator ','

set output 'analog.png'
plot 'analog.csv' using 1:2 with lines title 'Reference', 'analog.csv' using 1:3 with lines title 'PLL', 'analog.csv' using 1:4 with lines title 'PhaseSignal', 'analog.csv' using 1:5 with lines title 'PLLVoltage'

set output 'flipflop.png'
plot 'flipflop.csv' using 1:2 with lines title 'Reference', 'flipflop.csv' using 1:3 with lines title 'PLL', 'flipflop.csv' using 1:4 with lines title 'PhaseSignal', 'flipflop.csv' using 1:5 with lines title 'PLLVoltage'

set output 'type1.png'
plot 'type1.csv' using 1:2 with lines title 'Reference', 'type1.csv' using 1:3 with lines title 'PLL', 'type1.csv' using 1:4 with lines title 'PhaseSignal', 'type1.csv' using 1:5 with lines title 'PLLVoltage'

set output 'type2.png'
plot 'type2.csv' using 1:2 with lines title 'Reference', 'type2.csv' using 1:3 with lines title 'PLL', 'type2.csv' using 1:4 with lines title 'PhaseSignal', 'type2.csv' using 1:5 with lines title 'PLLVoltage'

set output 'phase.png'
plot 'phase.csv' using 1:2 with lines title 'Reference', 'phase.csv' using 1:3 with lines title 'PLL', 'phase.csv' using 1:4 with lines title 'PhaseSignal', 'phase.csv' using 1:5 with lines title 'PLLVoltage'

set output 'direct.png'
plot 'direct.csv' using 1:2 with lines title 'Reference', 'direct.csv' using 1:3 with lines title 'PLL', 'direct.csv' using 1:4 with lines title 'PhaseSignal', 'direct.csv' using 1:5 with lines title 'PLLVoltage'

set xrange[2000:5000]
set output 'flipfloplate.png'
plot 'flipflop.csv' using 1:2 with lines title 'Reference', 'flipflop.csv' using 1:3 with lines title 'PLL', 'flipflop.csv' using 1:4 with lines title 'PhaseSignal', 'flipflop.csv' using 1:5 with lines title 'PLLVoltage'

set output 'type1late.png'
plot 'type1.csv' using 1:2 with lines title 'Reference', 'type1.csv' using 1:3 with lines title 'PLL', 'type1.csv' using 1:4 with lines title 'PhaseSignal', 'type1.csv' using 1:5 with lines title 'PLLVoltage'


set output 'type2late.png'
plot 'type2.csv' using 1:2 with lines title 'Reference', 'type2.csv' using 1:3 with lines title 'PLL', 'type2.csv' using 1:4 with lines title 'PhaseSignal', 'type2.csv' using 1:5 with lines title 'PLLVoltage'
EOF

ffmpeg -y -f s16le -ar 48k -ac 2 -i analog.raw -b:a 192k analog.raw
ffmpeg -y -f s16le -ar 48k -ac 2 -i flipflop.raw -b:a 192k flipflop.mp3
ffmpeg -y -f s16le -ar 48k -ac 2 -i type1.raw -b:a 192k type1.mp3
ffmpeg -y -f s16le -ar 48k -ac 2 -i type2.raw -b:a 192k type2.mp3
ffmpeg -y -f s16le -ar 48k -ac 2 -i direct.raw -b:a 192k direct.mp3
ffmpeg -y -f s16le -ar 48k -ac 2 -i phase.raw -b:a 192k phase.mp3




