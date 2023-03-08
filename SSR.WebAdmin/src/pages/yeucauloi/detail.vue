<script>
import Layout from "../../layouts/main1";
import appConfig from "@/app.config";
import Multiselect from "vue-multiselect";
import { yeucauloiModel } from "@/models/yeucauloiModel";
export default {
  page: {
    title: "Thông tin chi tiết",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: {
    Layout
  },
  data() {
    return {
      title: "Thông tin chi tiết",
      model: yeucauloiModel.baseJson(),
      submitted: false,
      dateString: 'null',
      isShow: false,
      responseData: {
        resultString: null,
        resultCode: null
      },
      showDeleteModal: false
    };
  },
  validations: {
    model: {
    },
  },
  async created() {
    if (this.$route.params.id) {
      this.getById(this.$route.params.id);
    } else {
      this.model = yeucauloiModel.baseJson();
    }
  },
  mounted() {
    
  },
  methods: {
    fnGetList() {
      this.$refs.tblList?.refresh()
    },
    handleShowNotify(res) {
      this.isShow = true;
      this.responseData.resultCode = res.resultCode;
      this.responseData.resultString = res.resultString;
    },
    async getById(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data;
          this.dateString = this.model.dueDate.substring(0, 10);
        }
      });
    },
  }
};
</script>

<template>
  <Layout>
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold cs-title-page">Thông tin chi tiết</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button variant="primary" type="button" class="btn w-md btn-primary" @click="$router.go(-1)" size="sm">
                  <i class="mdi mdi-keyboard-backspace me-1"></i> Trở về
                </b-button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <form @submit.prevent="handleSubmit" ref="formContainer">    
              <div class="row">
                <div class="col-md-7">
                  <div class="col-lg-12 col-md-12 col-12">
                    <div class="mb-2">
                      <label class="form-label cs-title-form" for="validationCustom01"> Tiêu đề: {{ model.title }}</label>                                           
                    </div>
                    <div class="col-md-12 col-lg-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01">Mô tả:</label>
                        {{model.description}}
                      </div>
                    </div>
                  </div>
                </div>
                <div class="col-md-5">
                  <div class="row">
                    <!-- <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Đơn vị </label>
                        <treeselect :options="treeView" :value="modeldonvi.ParentId" :searchable="true" :show-count="true"
                          :default-expand-level="1" placeholder="Chọn đơn vị" v-model="model.Donvi">
                          <label slot="option-label"
                            slot-scope="{ node, shouldShowCount, count, labelClassName, countClassName }"
                            :class="labelClassName">
                            {{ node.label }}
                            <span v-if="shouldShowCount" :class="countClassName">({{ count }})</span>
                          </label>
                        </treeselect>
                      </div>
                    </div> -->
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Phân công theo nhóm </label>
                        <div v-for="(value , index) in model.assignee" :key="index">
                        <input id="validationCustom01" v-model="model.name" type="text" class="form-control" />
                      </div>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Nhãn yêu cầu lỗi </label>
                        <div v-for="(value , index) in model.label" :key="index">
                          <div class="mb-2">
                          <span class="colorstyle" v-bind:style="{ background: value.color }">{{ value.name }}</span>
                        </div>
                        </div>
                      </div>
                    </div>

                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Ngày đáo hạn: </label>
                        {{dateString}}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </form>
          </div>
          <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit
                        class="btn btn-outline-info m-1"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit
                        class="btn btn-danger btn m-1"
                        v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
        </div>
      </div>
    </div>
    
  </Layout>
</template>
<style scoped>
.title-capso {
  font-weight: bold;
  color: #00568C;

}

.content-capso {
  color: #00568C;
  padding-left: 10px;
}

.capso-container {
  margin-top: 10px;
  display: flex;
  padding: 0px;
}

.hidden-sortable:after,
.hidden-sortable:before {
  display: none !important;
}
</style>
<style lang="scss">
   .colorstyle {
  padding: 5px;
  border-radius: 5px;
  //font-weight: bold;
  color: white
}
.success-checkmark {
  width: 80px;
  height: 115px;
  margin: 0 auto;

  .check-icon {
    width: 80px;
    height: 80px;
    position: relative;
    border-radius: 50%;
    box-sizing: content-box;
    border: 4px solid #4CAF50;

    &::before {
      top: 3px;
      left: -2px;
      width: 30px;
      transform-origin: 100% 50%;
      border-radius: 100px 0 0 100px;
    }

    &::after {
      top: 0;
      left: 30px;
      width: 60px;
      transform-origin: 0 50%;
      border-radius: 0 100px 100px 0;
      animation: rotate-circle 4.25s ease-in;
    }

    &::before,
    &::after {
      content: '';
      height: 100px;
      position: absolute;
      background: #FFFFFF;
      transform: rotate(-45deg);
    }

    .icon-line {
      height: 5px;
      background-color: #4CAF50;
      display: block;
      border-radius: 2px;
      position: absolute;
      z-index: 10;

      &.line-tip {
        top: 46px;
        left: 14px;
        width: 25px;
        transform: rotate(45deg);
        animation: icon-line-tip 0.75s;
      }

      &.line-long {
        top: 38px;
        right: 8px;
        width: 47px;
        transform: rotate(-45deg);
        animation: icon-line-long 0.75s;
      }
    }

    .icon-circle {
      top: -4px;
      left: -4px;
      z-index: 10;
      width: 80px;
      height: 80px;
      border-radius: 50%;
      position: absolute;
      box-sizing: content-box;
      border: 4px solid rgba(76, 175, 80, .5);
    }

    .icon-fix {
      top: 8px;
      width: 5px;
      left: 26px;
      z-index: 1;
      height: 85px;
      position: absolute;
      transform: rotate(-45deg);
      background-color: #FFFFFF;
    }
 
  }
}

@keyframes rotate-circle {
  0% {
    transform: rotate(-45deg);
  }

  5% {
    transform: rotate(-45deg);
  }

  12% {
    transform: rotate(-405deg);
  }

  100% {
    transform: rotate(-405deg);
  }
}

@keyframes icon-line-tip {
  0% {
    width: 0;
    left: 1px;
    top: 19px;
  }

  54% {
    width: 0;
    left: 1px;
    top: 19px;
  }

  70% {
    width: 50px;
    left: -8px;
    top: 37px;
  }

  84% {
    width: 17px;
    left: 21px;
    top: 48px;
  }

  100% {
    width: 25px;
    left: 14px;
    top: 45px;
  }
}

@keyframes icon-line-long {
  0% {
    width: 0;
    right: 46px;
    top: 54px;
  }

  65% {
    width: 0;
    right: 46px;
    top: 54px;
  }

  84% {
    width: 55px;
    right: 0px;
    top: 35px;
  }

  100% {
    width: 47px;
    right: 8px;
    top: 38px;
  }
}

.btn-modal {
  background: #00568C;
  border: none;
  color: #fff;
}

.btn-modal:hover {
  background: #00568C;
  border: none;
  color: #fff;
  box-shadow: rgba(50, 50, 93, 0.25) 0px 13px 27px -5px, rgba(0, 0, 0, 0.3) 0px 8px 16px -8px;
}

#my-strictly-unique-vue-upload-multiple-image {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>
