<template>
<div class="vertical-menu">
    <div class="h-100" data-simplebar="init">
        <div class="simplebar-wrapper" style="margin: 0px;">
            <div class="simplebar-height-auto-observer-wrapper">
                <div class="simplebar-height-auto-observer"></div>
            </div> 
            <div class="simplebar-mask">
                <div class="simplebar-offset" style="right: 0px; bottom: 0px;">
                    <div class="simplebar-content-wrapper" tabindex="0" role="region" aria-label="scrollable content" style="height: 100%; overflow: hidden;">
                        <div class="simplebar-content" style="padding: 0px;">
                            <div id="sidebar-menu" class="p-0">
                                <ul id="side-menu" class="metismenu list-unstyled">
                                    <li>                                       
                                        <a href="/:slug/danh-sach-yeu-cau-loi" class="side-nav-link router-link-active">
                                            <i class="bx bx ti-home"></i>
                                            <span>Danh sách Issue</span>                                        
                                        </a>
                                    </li>
                                    <li>
                                        <a href="/du-an" class="side-nav-link">
                                            <i class="bx ti-agenda"></i>
                                            <span>Thông tin dự án</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="/du-an" class="side-nav-link">
                                            <i class="bx ti-agenda"></i>
                                            <span>Phân quyền</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="/du-an" class="side-nav-link">
                                            <i class="bx ti-agenda"></i>
                                            <span>Nhãn</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="/du-an" class="side-nav-link">
                                            <i class="bx ti-agenda"></i>
                                            <span>Thống kê</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="/du-an" class="side-nav-link">
                                            <i class="bx ti-agenda"></i>
                                            <span>Bước thực hiện</span>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div> 
            <div class="simplebar-placeholder" style="width: auto; height: 199px;"></div>
        </div> 
        <div class="simplebar-track simplebar-horizontal" style="visibility: hidden;">
            <div class="simplebar-scrollbar" style="width: 0px; display: none;"></div>
        </div> 
        <div class="simplebar-track simplebar-vertical" style="visibility: hidden;">
            <div class="simplebar-scrollbar" style="height: 0px; display: none;"></div>
        </div>
    </div>
</div>
</template>