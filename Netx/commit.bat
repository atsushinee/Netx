@echo off
for /l %%i in (1,1,28) do (
    date 2021/2/%%i
    echo 2021/2/%%i > .Netx
    git add .
    git commit -m "auto commit date"
)
git push
