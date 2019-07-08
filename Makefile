DOTNET			:= dotnet

DIR_NETCOREAPP	:= netcoreapp2.1
DIR_DEBUG		:= bin/Debug/$(DIR_NETCOREAPP)
DIR_RELEASE		:= bin/Release/$(DIR_NETCOREAPP)

TRG_TESTS		:= Tests
DIR_TESTS		:= Tests

.PHONY: all
all: release

.PHONY: release
release:
	$(DOTNET) build -c Release $(TARGET)

.PHONY: debug
debug:
	$(DOTNET) build -c Debug $(TARGET)

.PHONY: clean
clean:
	$(DOTNET) clean

.PHONY: profile
profile: TARGET := $(TRG_TESTS)
profile: release
	$(DOTNET) $(DIR_TESTS)/$(DIR_RELEASE)/$(TRG_TESTS).dll
