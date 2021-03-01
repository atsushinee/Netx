@echo off
for /l %%i in (1,1,31) do (
    date 2021/3/%%i
    echo 2021/3/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
git push
