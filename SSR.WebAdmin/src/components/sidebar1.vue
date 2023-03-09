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
                                            <a :href="`/${slug}/chi-tiet`" class="side-nav-link router-link-active">
                                                <img style="height: 30px;width: 30px;" src="@/assets/images/users/user-4.jpg" class="flex-shrink-0 me-3 rounded mx-auto d-blocks avatar-sm">
                                                <span>{{ model.name }} </span>                                        
                                            </a>
                                        </li>
                                        <li>                                       
                                            <a :href="`/${slug}/danh-sach-yeu-cau-loi`" class="side-nav-link router-link-active">
                                                <i class="mdi mdi-alert-outline"></i>
                                                <span>Danh sách yêu cầu lỗi</span>     
                                                                                   
                                            </a>
                                        </li> 
                                    <li>
                                        <a href="/du-an" class="side-nav-link">
                                            <i class="bx ti-agenda"></i>
                                            <span>Phân quyền</span>
                                        </a>
                                    </li>
                                    <router-link :to='`/${slug}/nhan-du-an`'>
                                    <li>
                                        <a href="/Label" class="side-nav-link">
                                            <i class="fas fa-tasks"></i>
                                            <span>Hoạt động</span>
                                        </a>
                                    </li>
                                    </router-link>
                                    <router-link :to='`/${slug}/thong-ke`'>
                                    <li>
                                        <a  class="side-nav-link">
                                            <i class="mdi mdi-file-chart"></i>
                                            <span>Thống kê</span>
                                        </a>
                                    </li>
                                    </router-link>
                                    <router-link :to='`/${slug}/buoc-thuc-hien`'>
                                    <li>
                                        <a class="side-nav-link">
                                            <i class="mdi mdi-debug-step-into"></i>
                                            <span>Bước thực hiện</span>
                                        </a>
                                    </li>
                                    </router-link>
                                    <li>
                                        <router-link :to='`/${slug}/thong-tin`' :class="side-nav-link">
                                        
                                            <i class="mdi mdi-cog"></i>
                                            <span>Cài đặt dự án</span>
                                        </router-link>
                                    </li> 
                                </ul>
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
    </div>
    </template>
    
    <script>
    import {projectModel} from "@/models/projectModel";
    export default {
        data() {
            return {
                slug: null,
                model: projectModel.baseJson()
            }
        },
        async created(){
            let currentProjectLocal = localStorage.getItem('currentProject');
            this.slug = JSON.parse(currentProjectLocal)
            if (!this.slug){
            this.$router.push(`/${this.slug}/du-an`)
            }
            if (this.$route.params.slug) {
            this.getBySlug(this.$route.params.slug);
            } else {
            this.model = projectModel.baseJson();
        }
            },
        methods:{
            async getBySlug(slug) {
          await this.$store.dispatch("projectStore/getBySlug", slug).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.model = res.data
            }
          });
          
        },
        handleCount() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      //lấy ds yêu cầu
      this.$store.dispatch("yeucauloiStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listIssue = res.data;
        }
      });
      
      //lấy ds dự án
      this.$store.dispatch("projectStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listProject = res.data;
        }
      });

      this.nameproject = JSON.parse(currentProjectLocal); //lưu tên dự án đang mở

      //tìm tên dự án đang mở trong listproject để lấy id
      const project = (this.listProject||[]).find(p => p.slug === this.nameproject)
      if (project) {
        this.idproject = project.id; //chứa idproject đang mở
        console.log(project)
      }
      else {
        this.idproject = null;
      }
      
      if (!Array.isArray(this.listIssue)){
        return [];
      }
      
      let count = this.listIssue.filter(p => p.projectId === this.idproject).length;
      console.log(count);
      if(count===[]){
        return count="0";
      }
      else{
        return count; 
      }

            
    },
        }
    }
    
    </script>